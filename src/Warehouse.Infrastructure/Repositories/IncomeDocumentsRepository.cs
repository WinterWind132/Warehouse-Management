using Application.DTO;
using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IncomeDocumentRepository : IIncomeDocumentRepository
    {
        private readonly WarehouseDbContext _context;

        public IncomeDocumentRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IncomeDocument> GetByIdAsync(Guid id)
        {
            var dataModel = await _context.IncomeDocuments
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.Resource)
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.UnitOfMeasure)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            return dataModel.Adapt<IncomeDocument>();
        }

        public async Task<IEnumerable<IncomeDocument>> GetAllAsync()
        {
            var dataModels = await _context.IncomeDocuments
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.Resource)
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.UnitOfMeasure)
                .AsNoTracking()
                .ToListAsync();

            return dataModels.Adapt<IEnumerable<IncomeDocument>>();
        }

        public async Task<IEnumerable<IncomeDocument>> GetAllAsync(IncomeDocumentFilterDto filter)
        {
            var query = _context.IncomeDocuments
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.Resource)
                .Include(d => d.IncomeResources)
                    .ThenInclude(ir => ir.UnitOfMeasure)
                .AsQueryable();
            
            if (filter.StartDate.HasValue)
            {
                query = query.Where(d => d.DocumentDate >= filter.StartDate.Value);
            }
            if (filter.EndDate.HasValue)
            {
                query = query.Where(d => d.DocumentDate <= filter.EndDate.Value);
            }
            if (filter.DocumentNumbers != null && filter.DocumentNumbers.Any())
            {
                query = query.Where(d => filter.DocumentNumbers.Contains(d.DocumentNumber));
            }
            if (filter.ResourceIds != null && filter.ResourceIds.Any())
            {
                query = query.Where(d => d.IncomeResources.Any(ir => filter.ResourceIds.Contains(ir.ResourceId)));
            }

            var dataModels = await query.ToListAsync();
            return dataModels.Adapt<IEnumerable<IncomeDocument>>();
        }

        public async Task AddAsync(IncomeDocument document)
        {
            var dataModel = document.Adapt<IncomeDocumentDataModel>();
            _context.IncomeDocuments.Add(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IncomeDocument document)
        {
            var dataModel = document.Adapt<IncomeDocumentDataModel>();
            _context.IncomeDocuments.Update(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var document = await _context.IncomeDocuments.FindAsync(id);
            if (document != null)
            {
                _context.IncomeDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsResourceUsedAsync(Guid resourceId)
        {
            return await _context.IncomeResources.AnyAsync(ir => ir.ResourceId == resourceId);
        }

        public async Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId)
        {
            return await _context.IncomeResources.AnyAsync(ir => ir.UnitOfMeasureId == unitOfMeasureId);
        }

        public async Task<bool> ExistsWithNumberAsync(string documentNumber)
        {
            return await _context.IncomeDocuments.AnyAsync(d => d.DocumentNumber == documentNumber);
        }
    }
}