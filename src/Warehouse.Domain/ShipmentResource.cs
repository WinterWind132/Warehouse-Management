namespace Domain;

public class ShipmentResource
{
    public Guid Id { get; set; }

    public ShipmentDocument ShipmentDocument { get; set; }
    public Resource Resource { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }

    public decimal Quantity { get; set; }
}