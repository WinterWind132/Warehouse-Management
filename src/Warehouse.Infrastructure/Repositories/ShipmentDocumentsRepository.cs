using Application.DTO;
using Application.Interfaces;
using Domain;
using Infrastructure.DataModels;
using Infrastructure.Presistence;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShipmentDocumentsRepository : IShipmentDocumentRepository
    {
        private readonly WarehouseDbContext _context;

        public ShipmentDocumentsRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<ShipmentDocument> GetByIdAsync(Guid id)
        {
            var dataModel = await _context.ShipmentDocuments
                .Include(d => d.Client)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(sr => sr.Resource)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(sr => sr.UnitOfMeasure)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            return dataModel.Adapt<ShipmentDocument>();
        }

        public async Task<IEnumerable<ShipmentDocument>> GetAllAsync()
        {
            var dataModels = await _context.ShipmentDocuments
                .Include(d => d.Client)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(sr => sr.Resource)
                .Include(d => d.ShipmentResources)
                    .ThenInclude(sr => sr.UnitOfMeasure)
                .AsNoTracking()
                .ToListAsync();

            return dataModels.Adapt<IEnumerable<ShipmentDocument>>();
        }

        public async Task AddAsync(ShipmentDocument document)
        {
            var dataModel = document.Adapt<ShipmentDocumentDataModel>();
            _context.ShipmentDocuments.Add(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ShipmentDocument document)
        {
            var dataModel = document.Adapt<ShipmentDocumentDataModel>();
            _context.ShipmentDocuments.Update(dataModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var document = await _context.ShipmentDocuments.FindAsync(id);
            if (document != null)
            {
                _context.ShipmentDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsResourceUsedAsync(Guid resourceId)
        {
            return await _context.ShipmentResources.AnyAsync(sr => sr.ResourceId == resourceId);
        }

        public async Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId)
        {
            return await _context.ShipmentResources.AnyAsync(sr => sr.UnitOfMeasureId == unitOfMeasureId);
        }

        public async Task<bool> IsClientUsedAsync(Guid clientId)
        {
            return await _context.ShipmentDocuments.AnyAsync(d => d.ClientId == clientId);
        }

        public async Task<bool> ExistsWithNumberAsync(string documentNumber)
        {
            return await _context.ShipmentDocuments.AnyAsync(d => d.DocumentNumber == documentNumber);
        }
    }
} 