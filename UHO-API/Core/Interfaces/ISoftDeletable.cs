
namespace UHO_API.Core.Interfaces;

public interface ISoftDeletable
{
     bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}