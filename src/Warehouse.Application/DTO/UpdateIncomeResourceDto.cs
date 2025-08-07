namespace Application.DTO;

public class UpdateIncomeResourceDto
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal Quantity { get; set; }
}