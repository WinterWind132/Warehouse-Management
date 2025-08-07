using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly WarehouseDbContext _context;

    public ClientRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<Client> GetByIdAsync(Guid id)
    {
        var dataModel = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return dataModel.Adapt<Client>();
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var dataModels = await _context.Clients.ToListAsync();
        return dataModels.Adapt<IEnumerable<Client>>();
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _context.Clients.AnyAsync(c => c.Name == name);
    }

    public async Task AddAsync(Client client)
    {
        var dataModel = client.Adapt<ClientDataModel>();
        _context.Clients.Add(dataModel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        var dataModel = client.Adapt<ClientDataModel>();
        _context.Clients.Update(dataModel);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var dataModel = await _context.Clients.FindAsync(id);
        if (dataModel != null)
        {
            _context.Clients.Remove(dataModel);
            await _context.SaveChangesAsync();
        }
    }
}