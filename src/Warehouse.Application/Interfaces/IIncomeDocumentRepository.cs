using Application.DTO;
using Domain;

namespace Application.Interfaces;

public interface IIncomeDocumentRepository
{
    Task<IncomeDocument> GetByIdAsync(Guid id);
    Task<IEnumerable<IncomeDocument>> GetAllAsync();
    Task<bool> ExistsWithNumberAsync(string documentNumber);
    Task<bool> IsResourceUsedAsync(Guid resourceId);
    Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId);
    Task AddAsync(IncomeDocument incomeDocument);
    Task UpdateAsync(IncomeDocument incomeDocument);
    Task DeleteAsync(Guid id);
}