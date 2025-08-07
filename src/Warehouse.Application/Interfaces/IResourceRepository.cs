using Domain;

namespace Application.Interfaces;

public interface IResourceRepository
{
    Task<Resource> GetByIdAsync(Guid id);
    Task<IEnumerable<Resource>> GetAllAsync();
    Task<bool> ExistsWithNameAsync(string name);
    Task AddAsync(Resource resource);
    Task UpdateAsync(Resource resource);
}