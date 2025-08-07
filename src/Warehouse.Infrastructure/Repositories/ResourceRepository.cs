using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly WarehouseDbContext _context;

    public ResourceRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<Resource> GetByIdAsync(Guid id)
    {
        var dataModel = await _context.Resources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return dataModel.Adapt<Resource>();
    }

    public async Task<IEnumerable<Resource>> GetAllAsync()
    {
        var dataModels = await _context.Resources.ToListAsync();
        return dataModels.Adapt<IEnumerable<Resource>>();
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _context.Resources.AnyAsync(r => r.Name == name);
    }

    public async Task AddAsync(Resource resource)
    {
        var dataModel = resource.Adapt<ResourceDataModel>();
        _context.Resources.Add(dataModel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Resource resource)
    {
        var dataModel = resource.Adapt<ResourceDataModel>();
        _context.Resources.Update(dataModel);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var dataModel = await _context.Resources.FindAsync(id);
        if (dataModel != null)
        {
            _context.Resources.Remove(dataModel);
            await _context.SaveChangesAsync();
        }
    }
}