namespace UHO_API.Core.Interfaces;

public interface IStringEntity
{
    string Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}