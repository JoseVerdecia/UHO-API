namespace UHO_API.Endpoints;

public static class RolesEndpoints
{
    public static void MapRolesEndpoints(this WebApplication app)
    {
        var rolesGroup = app.MapGroup("roles");

        rolesGroup.MapGet("/{id:int}",GetRolById).WithName("GetRolById");
        rolesGroup.MapGet("/", GetAllRoles).WithName("GetAllRoles");
    }

    private static Task<IResult> GetAllRoles()
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> GetRolById( )
    {
        throw new NotImplementedException();
    }
}