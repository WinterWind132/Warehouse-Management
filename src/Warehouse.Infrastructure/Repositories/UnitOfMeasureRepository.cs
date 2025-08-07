using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UnitOfMeasureRepository : IUnitOfMeasureRepository
{
    private readonly WarehouseDbContext _context;

    public UnitOfMeasureRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<UnitOfMeasure> GetByIdAsync(Guid id)
    {
        var dataModel = await _context.UnitOfMeasures.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return dataModel.Adapt<UnitOfMeasure>();
    }

    public async Task<IEnumerable<UnitOfMeasure>> GetAllAsync()
    {
        var dataModels = await _context.UnitOfMeasures.ToListAsync();
        return dataModels.Adapt<IEnumerable<UnitOfMeasure>>();
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        return await _context.UnitOfMeasures.AnyAsync(u => u.Name == name);
    }

    public async Task AddAsync(UnitOfMeasure unitOfMeasure)
    {
        var dataModel = unitOfMeasure.Adapt<UnitOfMeasureDataModel>();
        _context.UnitOfMeasures.Add(dataModel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UnitOfMeasure unitOfMeasure)
    {
        var dataModel = unitOfMeasure.Adapt<UnitOfMeasureDataModel>();
        _context.UnitOfMeasures.Update(dataModel);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var dataModel = await _context.UnitOfMeasures.FindAsync(id);
        if (dataModel != null)
        {
            _context.UnitOfMeasures.Remove(dataModel);
            await _context.SaveChangesAsync();
        }
    }
}