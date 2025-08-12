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
        IncomeDocuments = new IncomeDocumentRepository(context);
        ShipmentDocuments = new ShipmentDocumentsRepository(context);
        Balances = new BalanceRepository(context);
    }

    public IResourceRepository Resources { get; private set; }
    public IUnitOfMeasureRepository UnitOfMeasures { get; private set; }
    public IClientRepository Clients { get; private set; }
    public IIncomeDocumentRepository IncomeDocuments { get; private set; }
    public IShipmentDocumentRepository ShipmentDocuments { get; private set; }
    public IBalanceRepository Balances { get; private set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}