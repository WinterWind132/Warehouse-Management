using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.UnitOfMeasures.Queries.GetUnitOfMeasures;

public record GetUnitOfMeasuresQuery : IRequest<Result<IEnumerable<UnitOfMeasureDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public class GetUnitOfMeasuresQueryHandler : IRequestHandler<GetUnitOfMeasuresQuery, Result<IEnumerable<UnitOfMeasureDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUnitOfMeasuresQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<UnitOfMeasureDto>>> Handle(GetUnitOfMeasuresQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfMeasures = await _unitOfWork.UnitOfMeasures.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                unitOfMeasures = unitOfMeasures.Where(u => u.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.IsActive.HasValue)
            {
                var state = request.IsActive.Value ? Domain.Enum.EntityState.Active : Domain.Enum.EntityState.Archived;
                unitOfMeasures = unitOfMeasures.Where(u => u.State == state);
            }
            
            var dtos = unitOfMeasures.Adapt<IEnumerable<UnitOfMeasureDto>>();
            return Result<IEnumerable<UnitOfMeasureDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UnitOfMeasureDto>>.Failure($"Ошибка при получении единиц измерения: {ex.Message}");
        }
    }
} 