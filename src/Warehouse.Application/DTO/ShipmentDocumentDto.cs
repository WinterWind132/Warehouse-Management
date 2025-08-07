using Domain.Enum;

namespace Application.DTO;

public class ShipmentDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; }
    public DateTime DocumentDate { get; set; }
    public ShipmentDocumentState State { get; set; }
    public ICollection<ShipmentResourceDto> ShipmentResources { get; set; }
}
