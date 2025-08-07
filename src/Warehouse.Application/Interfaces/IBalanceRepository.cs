using Application.DTO;
using Domain;

namespace Application.Interfaces;

public interface IBalanceRepository
{
    Task<IEnumerable<Balance>> GetAllAsync(BalanceFilterDto filter);
    Task<Balance> GetByResourceAndUnitAsync(Guid resourceId, Guid unitOfMeasureId);
    Task AddAsync(Balance balance);
    Task UpdateAsync(Balance balance);
    Task AddOrUpdateAsync(Guid resourceId, Guid unitOfMeasureId, decimal quantity);
}