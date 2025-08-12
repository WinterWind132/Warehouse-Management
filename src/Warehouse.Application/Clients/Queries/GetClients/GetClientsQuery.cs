using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.Clients.Queries.GetClients;

public record GetClientsQuery : IRequest<Result<IEnumerable<ClientDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<IEnumerable<ClientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ClientDto>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                clients = clients.Where(c => c.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                           c.Address.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.IsActive.HasValue)
            {
                var state = request.IsActive.Value ? Domain.Enum.EntityState.Active : Domain.Enum.EntityState.Archived;
                clients = clients.Where(c => c.State == state);
            }
            
            var dtos = clients.Adapt<IEnumerable<ClientDto>>();
            return Result<IEnumerable<ClientDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ClientDto>>.Failure($"Ошибка при получении клиентов: {ex.Message}");
        }
    }
} 