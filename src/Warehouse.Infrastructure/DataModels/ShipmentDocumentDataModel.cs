using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Infrastructure.DataModels;

[Table("ShipmentDocuments")]
public class ShipmentDocumentDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string DocumentNumber { get; set; }

    [ForeignKey("Client")]
    public Guid ClientId { get; set; }
    public ClientDataModel Client { get; set; }

    [Required]
    public DateTime DocumentDate { get; set; } = DateTime.UtcNow;

    public ShipmentDocumentState State { get; set; } = ShipmentDocumentState.Created;

    public ICollection<ShipmentResourceDataModel> ShipmentResources { get; set; }
}