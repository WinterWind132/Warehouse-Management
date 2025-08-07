using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Infrastructure.DataModels;

[Table("Resources")]
public class ResourceDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    public EntityState State { get; set; } = EntityState.Active;
}