using Domain.Enum;

namespace Application.DTO.Update;

public class UpdateResourceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public EntityState State { get; set; }
}