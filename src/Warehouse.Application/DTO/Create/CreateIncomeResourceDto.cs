namespace Application.DTO;

public class CreateIncomeResourceDto
{
    public Guid ResourceId { get; set; } 
    public Guid UnitOfMeasureId { get; set; } 
    public decimal Quantity { get; set; }
}