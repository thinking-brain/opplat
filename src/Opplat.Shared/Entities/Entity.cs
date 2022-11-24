namespace Opplat.Shared.Entities;

public interface IEntity
{
    
}

public class Entity : IEntity
{
    public Guid Id { get; set; }

    protected void AutoGenerateId()
    {
        Id = new Guid();
    }
}
