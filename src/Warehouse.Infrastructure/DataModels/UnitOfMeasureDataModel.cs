using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Infrastructure.DataModels;

[Table("UnitOfMeasures")]
public class UnitOfMeasureDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public EntityState State { get; set; } = EntityState.Active;
}