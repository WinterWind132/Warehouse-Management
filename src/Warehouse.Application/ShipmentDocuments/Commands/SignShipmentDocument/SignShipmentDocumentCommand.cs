using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.ShipmentDocuments.Commands.SignShipmentDocument;

public record SignShipmentDocumentCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class SignShipmentDocumentCommandHandler : IRequestHandler<SignShipmentDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SignShipmentDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SignShipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _unitOfWork.ShipmentDocuments.GetByIdAsync(request.Id);
            if (document == null)
            {
                return Result.Failure("Документ отгрузки не найден.");
            }

            if (document.State == Domain.Enum.ShipmentDocumentState.Signed)
            {
                return Result.Failure("Документ уже подписан.");
            }

            if (document.State == Domain.Enum.ShipmentDocumentState.Canceled)
            {
                return Result.Failure("Нельзя подписать отмененный документ.");
            }

            // Проверяем достаточность ресурсов на складе
            foreach (var shipmentResource in document.ShipmentResources)
            {
                var balance = await _unitOfWork.Balances.GetByResourceAndUnitAsync(
                    shipmentResource.Resource.Id, 
                    shipmentResource.UnitOfMeasure.Id);

                if (balance == null || balance.Quantity < shipmentResource.Quantity)
                {
                    var availableQuantity = balance?.Quantity ?? 0;
                    return Result.Failure(
                        $"Недостаточно ресурса '{shipmentResource.Resource.Name}' в единицах '{shipmentResource.UnitOfMeasure.Name}'. " +
                        $"Доступно: {availableQuantity}, требуется: {shipmentResource.Quantity}");
                }
            }

            // Уменьшаем балансы на складе
            foreach (var shipmentResource in document.ShipmentResources)
            {
                await _unitOfWork.Balances.AddOrUpdateAsync(
                    shipmentResource.Resource.Id, 
                    shipmentResource.UnitOfMeasure.Id, 
                    -shipmentResource.Quantity); // Отрицательное значение для уменьшения
            }

            // Подписываем документ
            document.State = Domain.Enum.ShipmentDocumentState.Signed;
            await _unitOfWork.ShipmentDocuments.UpdateAsync(document);
            
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при подписании документа отгрузки: {ex.Message}");
        }
    }
} 