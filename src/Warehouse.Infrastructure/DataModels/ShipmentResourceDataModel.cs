using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataModels;

[Table("ShipmentResources")]
public class ShipmentResourceDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("ShipmentDocument")]
    public Guid ShipmentDocumentId { get; set; }
    public ShipmentDocumentDataModel ShipmentDocument { get; set; }

    [ForeignKey("Resource")]
    public Guid ResourceId { get; set; }
    public ResourceDataModel Resource { get; set; }

    [ForeignKey("UnitOfMeasure")]
    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureDataModel UnitOfMeasure { get; set; }

    [Required]
    public decimal Quantity { get; set; }
}