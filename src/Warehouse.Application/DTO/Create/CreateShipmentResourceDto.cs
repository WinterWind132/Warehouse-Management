namespace Application.DTO;

public class CreateShipmentResourceDto
{
    public Guid ResourceId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; }
}