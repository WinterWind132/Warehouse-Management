using Domain;

namespace Application.Interfaces;

public interface IShipmentDocumentRepository
{
    Task<ShipmentDocument> GetByIdAsync(Guid id);
    Task<IEnumerable<ShipmentDocument>> GetAllAsync(ShipmentDocumentFilterDto filter);
    Task AddAsync(ShipmentDocument document);
    Task UpdateAsync(ShipmentDocument document);
    Task DeleteAsync(Guid id);
    Task<bool> IsClientUsedAsync(Guid clientId);
    Task<bool> IsResourceUsedAsync(Guid resourceId);
    Task<bool> IsUnitOfMeasureUsedAsync(Guid unitOfMeasureId);
}