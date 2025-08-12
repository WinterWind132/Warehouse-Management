using Application.Common;
using Application.Interfaces;
using MediatR;

namespace Application.Clients.Commands.ArchiveClient;

public record ArchiveClientCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

public class ArchiveClientCommandHandler : IRequestHandler<ArchiveClientCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveClientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(request.Id);
            if (client == null)
            {
                return Result.Failure("Клиент не найден.");
            }
            
            var isUsedInShipment = await _unitOfWork.ShipmentDocuments.IsClientUsedAsync(request.Id);

            if (isUsedInShipment)
            {
                client.State = Domain.Enum.EntityState.Archived;
                await _unitOfWork.Clients.UpdateAsync(client);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                await _unitOfWork.Clients.DeleteAsync(request.Id); 
                await _unitOfWork.CompleteAsync();
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ошибка при архивировании клиента: {ex.Message}");
        }
    }
} 