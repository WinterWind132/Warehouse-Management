namespace Application.Interfaces;

public interface IUnitOfWork
{
    IResourceRepository Resources { get; }
    IUnitOfMeasureRepository UnitOfMeasures { get; }
    IClientRepository Clients { get; }
    IIncomeDocumentRepository IncomeDocuments { get; }
    IShipmentDocumentRepository ShipmentDocuments { get; }
    IBalanceRepository Balances { get; }
    Task<int> CompleteAsync();
}