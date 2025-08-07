using Domain.Enum;

namespace Domain;

public class Resource
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public EntityState State { get; set; }
}