using Domain.Enum;

namespace Application.DTO;

public class ClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public EntityState State { get; set; }
}