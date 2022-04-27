namespace Opplat.Shared.Entities;

public interface IEntity
{
    public string User { get; set; }
}

public interface IEntity<T>: IEntity
{
    public T Id { get; set; }
}

public class Entity : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string User { get; set; }

    protected void AutoGenerateId()
    {
        Id = new Guid();
    }
}
