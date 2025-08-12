using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;

namespace Application.ShipmentDocuments.Commands.UpdateShipmentDocument;

public record UpdateShipmentDocumentCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public DateTime DocumentDate { get; init; }
    public List<UpdateShipmentResourceDto> Resources { get; init; } = new();
}

public class UpdateShipmentDocumentCommandHandler : IRequestHandler<UpdateShipmentDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateShipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var existingDocument = await _unitOfWork.ShipmentDocuments.GetByIdAsync(request.Id);
            if (existingDocument == null)
            {
                return Result.Failure("Документ отгрузки не найден.");
            }

            // Проверяем что документ не подписан
            if (existingDocument.State == Domain.Enum.ShipmentDocumentState.Signed)
            {
                return Result.Failure("Нельзя редактировать подписанный документ.");
            }

            // Проверяем уникальность номера документа (если изменился)
            if (existingDocument.DocumentNumber != request.DocumentNumber)
            {
                var exists = await _unitOfWork.ShipmentDocuments.ExistsWithNumberAsync(request.DocumentNumber);
                if (exists)
                {
                    return Result.Failure($"Документ отгрузки с номером '{request.DocumentNumber}' уже существует.");
                }
            }

            // Проверяем существование клиента
            var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);
            if (client == null)
            {
                return Result.Failure("Клиент не найден.");
            }

            if (client.State == Domain.Enum.EntityState.Archived)
            {
                return Result.Failure("Нельзя использовать архивированного клиента.");
            }

            // Проверяем что документ не пустой
            if (!request.Resources.Any())
            {
                return Result.Failure("Документ отгрузки не может быть пустым.");
            }

            // Обновляем документ
            existingDocument.DocumentNumber = request.DocumentNumber;
            existingDocument.Client = client;
            existingDocument.DocumentDate = request.DocumentDate;
            existingDocument.ShipmentResources.Clear();

            // Добавляем новые ресурсы
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
                    ShipmentDocument = existingDocument,
                    Resource = resource,
                    UnitOfMeasure = unitOfMeasure,
                    Quantity = resourceDto.Quantity
                };
                
                existingDocument.ShipmentResources.Add(shipmentResource);
            }

            await _unitOfWork.ShipmentDocuments.UpdateAsync(existingDocument);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при обновлении документа отгрузки: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(UpdateShipmentDocumentCommand command)
    {
        var errors = new List<string>();
        
        if (command.Id == Guid.Empty)
            errors.Add("ID документа не может быть пустым");
        
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