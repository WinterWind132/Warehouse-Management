using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;

namespace Application.IncomeDocuments.Commands.CreateIncomeDocument;

public record CreateIncomeDocumentCommand : IRequest<Result>
{
    public string DocumentNumber { get; init; } = string.Empty;
    public DateTime DocumentDate { get; init; }
    public List<CreateIncomeResourceDto> Resources { get; init; } = new();
}

public class CreateIncomeDocumentCommandHandler : IRequestHandler<CreateIncomeDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateIncomeDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateIncomeDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            // Проверяем уникальность номера документа
            var exists = await _unitOfWork.IncomeDocuments.ExistsWithNumberAsync(request.DocumentNumber);
            if (exists)
            {
                return Result.Failure($"Документ поступления с номером '{request.DocumentNumber}' уже существует.");
            }

            // Создаем документ
            var document = new Domain.IncomeDocument
            {
                Id = Guid.NewGuid(),
                DocumentNumber = request.DocumentNumber,
                DocumentDate = request.DocumentDate,
                IncomeResources = new List<Domain.IncomeResource>()
            };

            // Добавляем ресурсы в документ
            foreach (var resourceDto in request.Resources)
            {
                var incomeResource = new Domain.IncomeResource
                {
                    Id = Guid.NewGuid(),
                    IncomeDocument = document,
                    ResourceId = resourceDto.ResourceId,
                    UnitOfMeasureId = resourceDto.UnitOfMeasureId,
                    Quantity = resourceDto.Quantity
                };
                
                document.IncomeResources.Add(incomeResource);
            }

            await _unitOfWork.IncomeDocuments.AddAsync(document);
            await _unitOfWork.CompleteAsync();

            // Обновляем балансы на складе
            foreach (var resourceDto in request.Resources)
            {
                await _unitOfWork.Balances.AddOrUpdateAsync(
                    resourceDto.ResourceId, 
                    resourceDto.UnitOfMeasureId, 
                    resourceDto.Quantity);
            }
            
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при создании документа поступления: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(CreateIncomeDocumentCommand command)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(command.DocumentNumber))
            errors.Add("Номер документа не может быть пустым");
        
        if (command.DocumentNumber.Length > 50)
            errors.Add("Номер документа не может быть длиннее 50 символов");
        
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