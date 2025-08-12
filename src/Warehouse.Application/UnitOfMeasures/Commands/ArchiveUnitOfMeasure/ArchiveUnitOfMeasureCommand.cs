using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.UnitOfMeasures.Commands.ArchiveUnitOfMeasure;

public record ArchiveUnitOfMeasureCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class ArchiveUnitOfMeasureCommandHandler : IRequestHandler<ArchiveUnitOfMeasureCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveUnitOfMeasureCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(request.Id);
            if (unitOfMeasure == null)
            {
                return Result.Failure("Единица измерения не найдена.");
            }
            
            var isUsedInIncome = await _unitOfWork.IncomeDocuments.IsUnitOfMeasureUsedAsync(request.Id);
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.IsUnitOfMeasureUsedAsync(request.Id);
            var isUsedInBalance = await _unitOfWork.Balances.IsUnitOfMeasureUsedAsync(request.Id);

            if (isUsedInIncome || isUsedInShipment || isUsedInBalance)
            {
                unitOfMeasure.State = Domain.Enum.EntityState.Archived;
                await _unitOfWork.UnitOfMeasures.UpdateAsync(unitOfMeasure);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                await _unitOfWork.UnitOfMeasures.DeleteAsync(request.Id); 
                await _unitOfWork.CompleteAsync();
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при архивировании единицы измерения: {ex.Message}");
        }
    }
} 