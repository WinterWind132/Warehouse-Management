using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.IncomeDocuments.Commands.DeleteIncomeDocument;

public record DeleteIncomeDocumentCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class DeleteIncomeDocumentCommandHandler : IRequestHandler<DeleteIncomeDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIncomeDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteIncomeDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _unitOfWork.IncomeDocuments.GetByIdAsync(request.Id);
            if (document == null)
            {
                return Result.Failure("Документ поступления не найден.");
            }

            // Проверяем достаточность ресурсов на складе для удаления
            foreach (var incomeResource in document.IncomeResources)
            {
                var balance = await _unitOfWork.Balances.GetByResourceAndUnitAsync(
                    incomeResource.ResourceId, 
                    incomeResource.UnitOfMeasureId);

                if (balance == null || balance.Quantity < incomeResource.Quantity)
                {
                    var resource = await _unitOfWork.Resources.GetByIdAsync(incomeResource.ResourceId);
                    var unitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(incomeResource.UnitOfMeasureId);
                    
                    var availableQuantity = balance?.Quantity ?? 0;
                    return Result.Failure(
                        $"Недостаточно ресурса '{resource?.Name}' в единицах '{unitOfMeasure?.Name}' для удаления документа. " +
                        $"Доступно: {availableQuantity}, требуется для удаления: {incomeResource.Quantity}");
                }
            }

            // Уменьшаем балансы на складе
            foreach (var incomeResource in document.IncomeResources)
            {
                await _unitOfWork.Balances.AddOrUpdateAsync(
                    incomeResource.ResourceId, 
                    incomeResource.UnitOfMeasureId, 
                    -incomeResource.Quantity); // Отрицательное значение для уменьшения
            }

            // Удаляем документ
            await _unitOfWork.IncomeDocuments.DeleteAsync(request.Id);
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при удалении документа поступления: {ex.Message}");
        }
    }
} 