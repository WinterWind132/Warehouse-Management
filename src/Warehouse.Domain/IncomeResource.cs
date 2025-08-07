namespace Domain;

public class IncomeResource
{
    public Guid Id { get; set; }

    public IncomeDocument IncomeDocument { get; set; }
    public Resource Resource { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }

    public decimal Quantity { get; set; }
}