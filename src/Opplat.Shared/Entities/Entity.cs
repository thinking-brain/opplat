namespace Opplat.Shared.Entities;

public interface IEntity
{
    public Guid Id { get; set; }
    public string User { get; set; }
}

public class Entity : IEntity
{
    public Guid Id { get; set; }
    public string User { get; set; }

    protected void AutoGenerateId()
    {
        Id = new Guid();
    }
}
