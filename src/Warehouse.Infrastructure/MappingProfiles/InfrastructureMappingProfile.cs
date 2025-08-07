using Domain;
using Infrastructure.DataModels;
using Mapster;

namespace Infrastructure.MappingProfiles;

public class InfrastructureMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ResourceDataModel, Resource>().TwoWays();
        config.NewConfig<UnitOfMeasureDataModel, UnitOfMeasure>().TwoWays();
        config.NewConfig<ClientDataModel, Client>().TwoWays();
        config.NewConfig<BalanceDataModel, Balance>().TwoWays();
        config.NewConfig<IncomeDocumentDataModel, IncomeDocument>().TwoWays();
        config.NewConfig<IncomeResourceDataModel, IncomeResource>().TwoWays();
        config.NewConfig<ShipmentDocumentDataModel, ShipmentDocument>().TwoWays();
        config.NewConfig<ShipmentResourceDataModel, ShipmentResource>().TwoWays();
    }
}