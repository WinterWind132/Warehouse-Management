namespace Application.DTO;

public class CreateIncomeDocumentDto
{
    public string DocumentNumber { get; set; }
    public DateTime DocumentDate { get; set; }
    public List<CreateIncomeResourceDto> IncomeResources { get; set; }
}