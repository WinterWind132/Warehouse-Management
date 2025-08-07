namespace Domain;

public class IncomeDocument
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; }
    public DateTime DocumentDate { get; set; }
    
    public ICollection<IncomeResource> IncomeResources { get; set; }
}