using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.ShipmentDocuments.Queries.GetShipmentDocuments;

public record GetShipmentDocumentsQuery : IRequest<Result<IEnumerable<ShipmentDocumentDto>>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public List<string> DocumentNumbers { get; init; } = new();
    public List<Guid> ResourceIds { get; init; } = new();
    public List<Guid> UnitOfMeasureIds { get; init; } = new();
}

public class GetShipmentDocumentsQueryHandler : IRequestHandler<GetShipmentDocumentsQuery, Result<IEnumerable<ShipmentDocumentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetShipmentDocumentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ShipmentDocumentDto>>> Handle(GetShipmentDocumentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _unitOfWork.ShipmentDocuments.GetAllAsync();

            // Фильтрация по датам
            if (request.FromDate.HasValue)
            {
                documents = documents.Where(d => d.DocumentDate >= request.FromDate.Value);
            }
            
            if (request.ToDate.HasValue)
            {
                documents = documents.Where(d => d.DocumentDate <= request.ToDate.Value);
            }

            // Фильтрация по номерам документов
            if (request.DocumentNumbers.Any())
            {
                documents = documents.Where(d => request.DocumentNumbers.Contains(d.DocumentNumber));
            }

            // Фильтрация по ресурсам
            if (request.ResourceIds.Any())
            {
                documents = documents.Where(d => d.ShipmentResources.Any(sr => request.ResourceIds.Contains(sr.Resource.Id)));
            }

            // Фильтрация по единицам измерения
            if (request.UnitOfMeasureIds.Any())
            {
                documents = documents.Where(d => d.ShipmentResources.Any(sr => request.UnitOfMeasureIds.Contains(sr.UnitOfMeasure.Id)));
            }
            
            var dtos = documents.Adapt<IEnumerable<ShipmentDocumentDto>>();
            return Result<IEnumerable<ShipmentDocumentDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ShipmentDocumentDto>>.Failure($"Ошибка при получении документов отгрузки: {ex.Message}");
        }
    }
} 