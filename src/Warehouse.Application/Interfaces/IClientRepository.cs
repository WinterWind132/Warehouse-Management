using Domain;

namespace Application.Interfaces;

public interface IClientRepository
{
    Task<Client> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<bool> ExistsWithNameAsync(string name);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    
    Task DeleteAsync(Guid id);
}