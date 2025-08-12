using Application.Common;
using Application.DTO;
using Application.Interfaces;
using MediatR;
using Mapster;

namespace Application.IncomeDocuments.Queries.GetIncomeDocuments;

public record GetIncomeDocumentsQuery : IRequest<Result<IEnumerable<IncomeDocumentDto>>>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public List<string> DocumentNumbers { get; init; } = new();
    public List<Guid> ResourceIds { get; init; } = new();
    public List<Guid> UnitOfMeasureIds { get; init; } = new();
}

public class GetIncomeDocumentsQueryHandler : IRequestHandler<GetIncomeDocumentsQuery, Result<IEnumerable<IncomeDocumentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIncomeDocumentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<IncomeDocumentDto>>> Handle(GetIncomeDocumentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _unitOfWork.IncomeDocuments.GetAllAsync();

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
                documents = documents.Where(d => d.IncomeResources.Any(ir => request.ResourceIds.Contains(ir.ResourceId)));
            }

            // Фильтрация по единицам измерения
            if (request.UnitOfMeasureIds.Any())
            {
                documents = documents.Where(d => d.IncomeResources.Any(ir => request.UnitOfMeasureIds.Contains(ir.UnitOfMeasureId)));
            }
            
            var dtos = documents.Adapt<IEnumerable<IncomeDocumentDto>>();
            return Result<IEnumerable<IncomeDocumentDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<IncomeDocumentDto>>.Failure($"Ошибка при получении документов поступления: {ex.Message}");
        }
    }
} 