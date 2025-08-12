using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;

namespace Application.IncomeDocuments.Commands.UpdateIncomeDocument;

public record UpdateIncomeDocumentCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public DateTime DocumentDate { get; init; }
    public List<UpdateIncomeResourceDto> Resources { get; init; } = new();
}

public class UpdateIncomeDocumentCommandHandler : IRequestHandler<UpdateIncomeDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncomeDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateIncomeDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateCommand(request);
            if (validationErrors.Any())
            {
                return Result.ValidationFailure(validationErrors);
            }

            var existingDocument = await _unitOfWork.IncomeDocuments.GetByIdAsync(request.Id);
            if (existingDocument == null)
            {
                return Result.Failure("Документ поступления не найден.");
            }

            // Проверяем уникальность номера документа (если изменился)
            if (existingDocument.DocumentNumber != request.DocumentNumber)
            {
                var exists = await _unitOfWork.IncomeDocuments.ExistsWithNumberAsync(request.DocumentNumber);
                if (exists)
                {
                    return Result.Failure($"Документ поступления с номером '{request.DocumentNumber}' уже существует.");
                }
            }

            // Уменьшаем балансы на старые количества
            foreach (var oldResource in existingDocument.IncomeResources)
            {
                await _unitOfWork.Balances.AddOrUpdateAsync(
                    oldResource.ResourceId, 
                    oldResource.UnitOfMeasureId, 
                    -oldResource.Quantity); // Отрицательное значение для уменьшения
            }

            // Обновляем документ
            existingDocument.DocumentNumber = request.DocumentNumber;
            existingDocument.DocumentDate = request.DocumentDate;
            existingDocument.IncomeResources.Clear();

            // Добавляем новые ресурсы
            foreach (var resourceDto in request.Resources)
            {
                var incomeResource = new Domain.IncomeResource
                {
                    Id = Guid.NewGuid(),
                    IncomeDocument = existingDocument,
                    ResourceId = resourceDto.ResourceId,
                    UnitOfMeasureId = resourceDto.UnitOfMeasureId,
                    Quantity = resourceDto.Quantity
                };
                
                existingDocument.IncomeResources.Add(incomeResource);
            }

            await _unitOfWork.IncomeDocuments.UpdateAsync(existingDocument);
            await _unitOfWork.CompleteAsync();

            // Увеличиваем балансы на новые количества
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
            return Result.Failure($"Ошибка при обновлении документа поступления: {ex.Message}");
        }
    }

    private List<string> ValidateCommand(UpdateIncomeDocumentCommand command)
    {
        var errors = new List<string>();
        
        if (command.Id == Guid.Empty)
            errors.Add("ID документа не может быть пустым");
        
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