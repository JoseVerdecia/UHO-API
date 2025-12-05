using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Infraestructure.Repository;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class AreaModel : IEntity,ISoftDeletable
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [ForeignKey("JefeAreaId")]
    public ApplicationUser? JefeArea { get; set; }
    
    public string? JefeAreaId { get; set; }
    
    // IEntity implementation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // ISoftDeletable implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}