using Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Presistence;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }

    public DbSet<ResourceDataModel> Resources { get; set; }
    public DbSet<UnitOfMeasureDataModel> UnitOfMeasures { get; set; }
    public DbSet<ClientDataModel> Clients { get; set; }
    public DbSet<BalanceDataModel> Balances { get; set; }
    public DbSet<IncomeDocumentDataModel> IncomeDocuments { get; set; }
    public DbSet<IncomeResourceDataModel> IncomeResources { get; set; }
    public DbSet<ShipmentDocumentDataModel> ShipmentDocuments { get; set; }
    public DbSet<ShipmentResourceDataModel> ShipmentResources { get; set; }
}