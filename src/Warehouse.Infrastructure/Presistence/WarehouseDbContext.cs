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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var kgId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var pcId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");
        modelBuilder.Entity<UnitOfMeasureDataModel>().HasData(
            new UnitOfMeasureDataModel { Id = kgId, Name = "Килограмм", State = Domain.Enum.EntityState.Active },
            new UnitOfMeasureDataModel { Id = pcId, Name = "Штука", State = Domain.Enum.EntityState.Active }
        );

        var waterId = Guid.NewGuid();
        var boltsId = Guid.NewGuid();
        modelBuilder.Entity<ResourceDataModel>().HasData(
            new ResourceDataModel { Id = waterId, Name = "Вода", State = Domain.Enum.EntityState.Active },
            new ResourceDataModel { Id = boltsId, Name = "Болты M10", State = Domain.Enum.EntityState.Active }
        );

        var clientAId = Guid.NewGuid();
        modelBuilder.Entity<ClientDataModel>().HasData(
            new ClientDataModel { Id = clientAId, Name = "Клиент А", Address = "ул. Примерная, 10", State = Domain.Enum.EntityState.Active }
        );

        modelBuilder.Entity<BalanceDataModel>().HasData(
            new BalanceDataModel { Id = Guid.NewGuid(), ResourceId = waterId, UnitOfMeasureId = kgId, Quantity = 1000m },
            new BalanceDataModel { Id = Guid.NewGuid(), ResourceId = boltsId, UnitOfMeasureId = pcId, Quantity = 500m }
        );
        
        var incomeDocId = Guid.NewGuid();
        modelBuilder.Entity<IncomeDocumentDataModel>().HasData(
            new IncomeDocumentDataModel { Id = incomeDocId, DocumentNumber = "INC-001", DocumentDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<IncomeResourceDataModel>().HasData(
            new IncomeResourceDataModel { Id = Guid.NewGuid(), IncomeDocumentId = incomeDocId, ResourceId = waterId, UnitOfMeasureId = kgId, Quantity = 200m }
        );
    }
}