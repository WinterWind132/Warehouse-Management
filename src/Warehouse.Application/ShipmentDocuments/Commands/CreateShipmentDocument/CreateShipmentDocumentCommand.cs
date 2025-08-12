using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;

namespace Application.ShipmentDocuments.Commands.CreateShipmentDocument;

public record CreateShipmentDocumentCommand : IRequest<Result>
{
    public string DocumentNumber { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public DateTime DocumentDate { get; init; }
    public List<CreateShipmentResourceDto> Resources { get; init; } = new();
}

public class CreateShipmentDocumentCommandHandler : IRequestHandler<CreateShipmentDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipmentDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateShipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            // Проверяем уникальность номера документа
            var exists = await _unitOfWork.ShipmentDocuments.ExistsWithNumberAsync(request.DocumentNumber);
            if (exists)
            {
                return Result.Failure($"Документ отгрузки с номером '{request.DocumentNumber}' уже существует.");
            }

            // Проверяем существование клиента
            var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);
            if (client == null)
            {
                return Result.Failure("Клиент не найден.");
            }

            if (client.State == Domain.Enum.EntityState.Archived)
            {
                return Result.Failure("Нельзя создать документ отгрузки для архивированного клиента.");
            }

            // Проверяем что документ не пустой
            if (!request.Resources.Any())
            {
                return Result.Failure("Документ отгрузки не может быть пустым.");
            }

            // Создаем документ
            var document = new Domain.ShipmentDocument
            {
                Id = Guid.NewGuid(),
                DocumentNumber = request.DocumentNumber,
                Client = client,
                DocumentDate = request.DocumentDate,
                State = Domain.Enum.ShipmentDocumentState.Created,
                ShipmentResources = new List<Domain.ShipmentResource>()
            };

            // Добавляем ресурсы в документ
            foreach (var resourceDto in request.Resources)
            {
                var resource = await _unitOfWork.Resources.GetByIdAsync(resourceDto.ResourceId);
                var unitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(resourceDto.UnitOfMeasureId);

                if (resource == null)
                {
                    return Result.Failure($"Ресурс с ID {resourceDto.ResourceId} не найден.");
                }

                if (unitOfMeasure == null)
                {
                    return Result.Failure($"Единица измерения с ID {resourceDto.UnitOfMeasureId} не найдена.");
                }

                if (resource.State == Domain.Enum.EntityState.Archived)
                {
                    return Result.Failure($"Нельзя использовать архивированный ресурс '{resource.Name}'.");
                }

                if (unitOfMeasure.State == Domain.Enum.EntityState.Archived)
                {
                    return Result.Failure($"Нельзя использовать архивированную единицу измерения '{unitOfMeasure.Name}'.");
                }

                var shipmentResource = new Domain.ShipmentResource
                {
                    Id = Guid.NewGuid(),
                    ShipmentDocument = document,
                    Resource = resource,
                    UnitOfMeasure = unitOfMeasure,
                    Quantity = resourceDto.Quantity
                };
                
                document.ShipmentResources.Add(shipmentResource);
            }

            await _unitOfWork.ShipmentDocuments.AddAsync(document);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при создании документа отгрузки: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(CreateShipmentDocumentCommand command)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(command.DocumentNumber))
            errors.Add("Номер документа не может быть пустым");
        
        if (command.DocumentNumber.Length > 50)
            errors.Add("Номер документа не может быть длиннее 50 символов");
        
        if (command.ClientId == Guid.Empty)
            errors.Add("ID клиента не может быть пустым");
        
        if (command.DocumentDate == default)
            errors.Add("Дата документа не может быть пустой");
        
        if (command.DocumentDate > DateTime.Now)
            errors.Add("Дата документа не может быть в будущем");
        
        // Проверяем ресурсы
        foreach (var resource in command.Resources)
        {
            if (resource.ResourceId == Guid.Empty)
                errors.Add("ID ресурса не может быть пустым");
            
            if (resource.UnitOfMeasureId == Guid.Empty)
                errors.Add("ID единицы измерения не может быть пустым");
            
            if (resource.Quantity <= 0)
                errors.Add("Количество должно быть больше нуля");
        }
        
        return errors;
    }
} 