using UHO_API.Infraestructure.Repository;

namespace UHO_API.Interfaces;

public interface ISoftDeletable
{
     bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}