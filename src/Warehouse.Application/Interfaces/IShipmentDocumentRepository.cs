using Application.DTO;
using Domain;

namespace Application.Interfaces;

public interface IShipmentDocumentRepository
{
    Task<ShipmentDocument> GetByIdAsync(Guid id);
    Task<IEnumerable<ShipmentDocument>> GetAllAsync();
    Task<bool> ExistsWithNumberAsync(string documentNumber);
    Task<bool> IsResourceUsedAsync(Guid resourceId);
    Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId);
    Task<bool> IsClientUsedAsync(Guid clientId);
    Task AddAsync(ShipmentDocument shipmentDocument);
    Task UpdateAsync(ShipmentDocument shipmentDocument);
    Task DeleteAsync(Guid id);
}