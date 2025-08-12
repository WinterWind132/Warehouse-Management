using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Resources.Commands.ArchiveResource;

public record ArchiveResourceCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class ArchiveResourceCommandHandler : IRequestHandler<ArchiveResourceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveResourceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveResourceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(request.Id);
            if (resource == null)
            {
                return Result.Failure("Ресурс не найден.");
            }
            
            var isUsedInIncome = await _unitOfWork.IncomeDocuments.IsResourceUsedAsync(request.Id);
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.IsResourceUsedAsync(request.Id);

            if (isUsedInIncome || isUsedInShipment)
            {
                resource.State = Domain.Enum.EntityState.Archived;
                await _unitOfWork.Resources.UpdateAsync(resource);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                await _unitOfWork.Resources.DeleteAsync(request.Id); 
                await _unitOfWork.CompleteAsync();
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при архивировании ресурса: {ex.Message}");
        }
    }
} 