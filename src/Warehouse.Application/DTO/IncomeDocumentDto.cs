namespace Application.DTO;

public class IncomeDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; }
    public DateTime DocumentDate { get; set; }
    public ICollection<IncomeResourceDto> IncomeResources { get; set; }
}