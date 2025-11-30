using Microsoft.AspNetCore.Identity;
using UHO_API.Interfaces;

namespace UHO_API.Models;

public class ApplicationUser : IdentityUser, ISoftDeletable
{
  
    public string FullName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}