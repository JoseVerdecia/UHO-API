using Microsoft.AspNetCore.Identity;

namespace UHO_API.Core.Entities;

public class ApplicationRole:IdentityRole
{
   
    public ApplicationRole() : base()
    {
    }
    
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}