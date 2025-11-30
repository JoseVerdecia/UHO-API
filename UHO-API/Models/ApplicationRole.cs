using Microsoft.AspNetCore.Identity;

namespace UHO_API.Models;

public class ApplicationRole:IdentityRole
{
   
    public ApplicationRole() : base()
    {
    }
    
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}