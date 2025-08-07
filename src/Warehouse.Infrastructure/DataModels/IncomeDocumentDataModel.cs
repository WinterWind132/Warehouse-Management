using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DataModels;

[Table("IncomeDocuments")]
public class IncomeDocumentDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string DocumentNumber { get; set; }

    [Required]
    public DateTime DocumentDate { get; set; } = DateTime.UtcNow;

    public ICollection<IncomeResourceDataModel> IncomeResources { get; set; }
}