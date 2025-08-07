namespace Application.DTO;

public class IncomeResourceDto
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; }
    public decimal Quantity { get; set; }
}