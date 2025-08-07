using Application.DTO;
using Domain;

namespace Application.Interfaces;

public interface IIncomeDocumentRepository
{
    Task<IncomeDocument> GetByIdAsync(Guid id);
    Task<IEnumerable<IncomeDocument>> GetAllAsync(IncomeDocumentFilterDto filter);
    Task AddAsync(IncomeDocument document);
    Task UpdateAsync(IncomeDocument document);
    Task DeleteAsync(Guid id);
    Task<bool> IsResourceUsedAsync(Guid resourceId);
    Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId);
}