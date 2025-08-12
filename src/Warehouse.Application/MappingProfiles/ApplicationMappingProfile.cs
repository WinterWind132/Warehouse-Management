using Application.DTO;
using Domain;
using Mapster;

namespace Application.MappingProfiles;

public class ApplicationMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateClientDto, Client>();

        config.NewConfig<CreateIncomeDocumentDto, IncomeDocument>()
            .Map(dest => dest.IncomeResources, 
                src => src.IncomeResources.Adapt<List<IncomeResource>>());

        config.NewConfig<CreateIncomeResourceDto, IncomeResource>();
        config.NewConfig<UpdateIncomeResourceDto, IncomeResource>();

        config.NewConfig<Resource, ResourceDto>();
        config.NewConfig<UnitOfMeasure, UnitOfMeasureDto>();
        config.NewConfig<Client, ClientDto>();
        config.NewConfig<Balance, BalanceDto>()
            .Map(dest => dest.ResourceName,
                src => src.Resource.Name)
            .Map(dest => dest.UnitOfMeasureName,
                src => src.UnitOfMeasure.Name);
            
        config.NewConfig<IncomeDocument, IncomeDocumentDto>()
            .Map(dest => dest.IncomeResources,
                src => src.IncomeResources.Adapt<List<IncomeResourceDto>>());

        config.NewConfig<IncomeResource, IncomeResourceDto>();
    }
}