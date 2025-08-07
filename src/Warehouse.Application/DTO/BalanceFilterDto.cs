namespace Application.DTO;

public class BalanceFilterDto
{
    public List<Guid> ResourceIds { get; set; }
    public List<Guid> UnitOfMeasureIds { get; set; }
}