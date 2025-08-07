namespace Application.DTO;

public class UpdateIncomeDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; }
    public DateTime DocumentDate { get; set; }
    public List<UpdateIncomeResourceDto> IncomeResources { get; set; }
}