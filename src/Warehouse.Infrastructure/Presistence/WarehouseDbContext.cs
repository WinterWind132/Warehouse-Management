using Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Presistence;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<ResourceDataModel> Resources => Set<ResourceDataModel>();
    public DbSet<UnitOfMeasureDataModel> UnitOfMeasures => Set<UnitOfMeasureDataModel>();
    public DbSet<ClientDataModel> Clients => Set<ClientDataModel>();
    public DbSet<BalanceDataModel> Balances => Set<BalanceDataModel>();
    public DbSet<IncomeDocumentDataModel> IncomeDocuments => Set<IncomeDocumentDataModel>();
    public DbSet<IncomeResourceDataModel> IncomeResources => Set<IncomeResourceDataModel>();
    public DbSet<ShipmentDocumentDataModel> ShipmentDocuments => Set<ShipmentDocumentDataModel>();
    public DbSet<ShipmentResourceDataModel> ShipmentResources => Set<ShipmentResourceDataModel>();
}