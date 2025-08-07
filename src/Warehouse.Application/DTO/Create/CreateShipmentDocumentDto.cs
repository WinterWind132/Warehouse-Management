namespace Application.DTO;

public class CreateShipmentDocumentDto
{
    public string DocumentNumber { get; set; }
    public Guid ClientId { get; set; }
    public DateTime DocumentDate { get; set; }
    public List<CreateShipmentResourceDto> ShipmentResources { get; set; }
}