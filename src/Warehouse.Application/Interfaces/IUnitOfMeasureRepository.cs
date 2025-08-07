using Domain;

namespace Application.Interfaces;

public interface IUnitOfMeasureRepository
{
    Task<UnitOfMeasure> GetByIdAsync(Guid id);
    Task<IEnumerable<UnitOfMeasure>> GetAllAsync();
    Task<bool> ExistsWithNameAsync(string name);
    Task AddAsync(UnitOfMeasure unitOfMeasure);
    Task UpdateAsync(UnitOfMeasure unitOfMeasure);
}