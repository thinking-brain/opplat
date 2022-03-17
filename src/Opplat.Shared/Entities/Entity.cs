namespace Opplat.Shared.Entities;

public interface IEntity
{

}

public interface IEntity<T>: IEntity
{
    public T Id { get; set; }
}

public class Entity : IEntity<Guid>
{
    public Guid Id { get; set; }

    protected void AutoGenerateId()
    {
        Id = new Guid();
    }
}
