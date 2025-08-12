using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.Balances.Queries.GetBalances;

public record GetBalancesQuery : IRequest<Result<IEnumerable<BalanceDto>>>
{
    public Guid? ResourceId { get; init; }
    public Guid? UnitOfMeasureId { get; init; }
}

public class GetBalancesQueryHandler : IRequestHandler<GetBalancesQuery, Result<IEnumerable<BalanceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBalancesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<BalanceDto>>> Handle(GetBalancesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filter = new BalanceFilterDto
            {
                ResourceIds = request.ResourceId.HasValue ? new List<Guid> { request.ResourceId.Value } : new List<Guid>(),
                UnitOfMeasureIds = request.UnitOfMeasureId.HasValue ? new List<Guid> { request.UnitOfMeasureId.Value } : new List<Guid>()
            };

            var balances = await _unitOfWork.Balances.GetAllAsync(filter);
            var dtos = balances.Adapt<IEnumerable<BalanceDto>>();
            
            return Result<IEnumerable<BalanceDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BalanceDto>>.Failure($"Ошибка при получении балансов: {ex.Message}");
        }
    }
} 