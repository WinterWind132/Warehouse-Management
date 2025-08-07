using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataModels;

[Table("Balances")]
public class BalanceDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("Resource")]
    public Guid ResourceId { get; set; }
    public ResourceDataModel Resource { get; set; }

    [ForeignKey("UnitOfMeasure")]
    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureDataModel UnitOfMeasure { get; set; }

    [Required]
    public decimal Quantity { get; set; }
}