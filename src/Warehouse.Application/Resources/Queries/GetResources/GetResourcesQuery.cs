using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.Resources.Queries.GetResources;

public record GetResourcesQuery : IRequest<Result<IEnumerable<ResourceDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
}

public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, Result<IEnumerable<ResourceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetResourcesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ResourceDto>>> Handle(GetResourcesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var resources = await _unitOfWork.Resources.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                resources = resources.Where(r => r.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.IsActive.HasValue)
            {
                var state = request.IsActive.Value ? Domain.Enum.EntityState.Active : Domain.Enum.EntityState.Archived;
                resources = resources.Where(r => r.State == state);
            }
            
            var dtos = resources.Adapt<IEnumerable<ResourceDto>>();
            return Result<IEnumerable<ResourceDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ResourceDto>>.Failure($"Ошибка при получении ресурсов: {ex.Message}");
        }
    }
} 