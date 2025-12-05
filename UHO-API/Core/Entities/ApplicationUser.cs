using Microsoft.AspNetCore.Identity;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class ApplicationUser : IdentityUser,IStringEntity,ISoftDeletable
{
  
    public string FullName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}