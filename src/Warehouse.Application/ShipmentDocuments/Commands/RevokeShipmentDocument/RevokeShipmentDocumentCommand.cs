using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.ShipmentDocuments.Commands.RevokeShipmentDocument;

public record RevokeShipmentDocumentCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class RevokeShipmentDocumentCommandHandler : IRequestHandler<RevokeShipmentDocumentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeShipmentDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RevokeShipmentDocumentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _unitOfWork.ShipmentDocuments.GetByIdAsync(request.Id);
            if (document == null)
            {
                return Result.Failure("Документ отгрузки не найден.");
            }

            if (document.State != Domain.Enum.ShipmentDocumentState.Signed)
            {
                return Result.Failure("Можно отозвать только подписанный документ.");
            }

            // Увеличиваем балансы на складе (возвращаем ресурсы)
            foreach (var shipmentResource in document.ShipmentResources)
            {
                await _unitOfWork.Balances.AddOrUpdateAsync(
                    shipmentResource.Resource.Id, 
                    shipmentResource.UnitOfMeasure.Id, 
                    shipmentResource.Quantity); // Положительное значение для увеличения
            }

            // Отзываем документ
            document.State = Domain.Enum.ShipmentDocumentState.Canceled;
            await _unitOfWork.ShipmentDocuments.UpdateAsync(document);
            
            await _unitOfWork.CompleteAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при отзыве документа отгрузки: {ex.Message}");
        }
    }
} 