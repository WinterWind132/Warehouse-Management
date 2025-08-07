using Application.Interfaces;
using Infrastructure.Presistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WarehouseDbContext _context;

    public UnitOfWork(WarehouseDbContext context)
    {
        _context = context;
        Resources = new ResourceRepository(context);
        UnitOfMeasures = new UnitOfMeasureRepository(context);
        Clients = new ClientRepository(context);
    }

    public IResourceRepository Resources { get; private set; }
    public IUnitOfMeasureRepository UnitOfMeasures { get; private set; }
    public IClientRepository Clients { get; private set; }
    public IIncomeDocumentRepository IncomeDocuments { get; }
    public IShipmentDocumentRepository ShipmentDocuments { get; }
    public IBalanceRepository Balances { get; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}