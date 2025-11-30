using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Interfaces;

namespace UHO_API.Models;

public class AreaModel : ISoftDeletable
{
    [Key]
    public int Id { get; set; }
    
    public string Nombre { get; set; }
    
    [ForeignKey("JefeAreaId")]
    public ApplicationUser? JefeArea { get; set; }
    
    public string? JefeAreaId { get; set; }
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}