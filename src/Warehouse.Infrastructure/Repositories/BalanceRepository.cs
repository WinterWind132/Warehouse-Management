using Application.DTO;
using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BalanceRepository : IBalanceRepository
    {
        private readonly WarehouseDbContext _context;

        public BalanceRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Balance>> GetAllAsync(BalanceFilterDto filter)
        {
            var query = _context.Balances
                .Include(b => b.Resource)
                .Include(b => b.UnitOfMeasure)
                .AsQueryable();

            if (filter.ResourceIds != null && filter.ResourceIds.Any())
            {
                query = query.Where(b => filter.ResourceIds.Contains(b.ResourceId));
            }
            if (filter.UnitOfMeasureIds != null && filter.UnitOfMeasureIds.Any())
            {
                query = query.Where(b => filter.UnitOfMeasureIds.Contains(b.UnitOfMeasureId));
            }

            var dataModels = await query.ToListAsync();
            return dataModels.Adapt<IEnumerable<Balance>>();
        }

        public async Task<Balance> GetByResourceAndUnitAsync(Guid resourceId, Guid unitOfMeasureId)
        {
            var dataModel = await _context.Balances
                .FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitOfMeasureId == unitOfMeasureId);
            return dataModel.Adapt<Balance>();
        }

        public async Task AddAsync(Balance balance)
        {
            var dataModel = balance.Adapt<BalanceDataModel>();
            _context.Balances.Add(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Balance balance)
        {
            var dataModel = balance.Adapt<BalanceDataModel>();
            _context.Balances.Update(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateAsync(Guid resourceId, Guid unitOfMeasureId, decimal quantity)
        {
            var existingBalance = await _context.Balances.FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.UnitOfMeasureId == unitOfMeasureId);

            if (existingBalance == null)
            {
                var newBalance = new BalanceDataModel
                {
                    ResourceId = resourceId,
                    UnitOfMeasureId = unitOfMeasureId,
                    Quantity = quantity
                };
                _context.Balances.Add(newBalance);
            }
            else
            {
                existingBalance.Quantity += quantity;
                _context.Balances.Update(existingBalance);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId)
        {
            return await _context.Balances.AnyAsync(b => b.UnitOfMeasureId == unitOfMeasureId);
        }
    }