namespace Opplat.Domain.Entities;

public interface IEntity
{

}

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }
}
