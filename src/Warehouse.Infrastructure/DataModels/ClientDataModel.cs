using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enum;

namespace Infrastructure.DataModels;

[Table("Clients")]
public class ClientDataModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Address { get; set; }

    public EntityState State { get; set; } = EntityState.Active;
}