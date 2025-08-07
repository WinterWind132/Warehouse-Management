using Domain.Enum;

namespace Domain;

public class UnitOfMeasure
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public EntityState State { get; set; }
}