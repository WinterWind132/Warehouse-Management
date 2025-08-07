using Domain.Enum;

namespace Domain;

public class ShipmentDocument
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; }

    public Client Client { get; set; }
    
    public DateTime DocumentDate { get; set; }
    public ShipmentDocumentState State { get; set; }

    public ICollection<ShipmentResource> ShipmentResources { get; set; }
}