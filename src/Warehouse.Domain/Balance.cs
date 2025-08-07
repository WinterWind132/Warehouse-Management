namespace Domain;

public class Balance
{
    public Guid Id { get; set; }
    
    public Resource Resource { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    
    public decimal Quantity { get; set; }
}