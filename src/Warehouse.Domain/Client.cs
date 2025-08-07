using Domain.Enum;

namespace Domain;

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public EntityState State { get; set; }
}