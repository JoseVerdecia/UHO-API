namespace UHO_API.Core.Interfaces;

public interface  IEntity
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}