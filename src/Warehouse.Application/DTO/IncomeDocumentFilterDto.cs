namespace Application.DTO;

public class IncomeDocumentFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> DocumentNumbers { get; set; }
    public List<Guid> ResourceIds { get; set; }
}