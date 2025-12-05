using Microsoft.AspNetCore.Mvc;
using UHO_API.Core.Entities;
using UHO_API.Core.Extensions;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Features.Users.Commands;
using UHO_API.Features.Users.Queries;
using UHO_API.Shared.Dtos;


namespace UHO_API.Features.Users.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("users");

        userGroup.MapGet("/", GetAllUsers);
        userGroup.MapGet("/{id:Guid}", GetUser);
        userGroup.MapGet("/users-in-rol-jefeArea", GetJefeAreas);
        userGroup.MapGet("/users-in-rol-jefeProceso", GetJefeProcesos);
        userGroup.MapGet("/users-in-rol-usuario-normal", GetUsuariosNormal);

        userGroup.MapPost("/degrade-user/{id:Guid}", DegradeUserToUsuarioNormal)
            .WithName("DegradeUser")
            .WithTags("Users")
            .WithSummary("Degrada un usuario a Rol : UsuarioNormal")
            .WithDescription("Quita los roles de 'JefeArea' o 'JefeProceso' a un usuario y le asigna el rol 'UsuarioNormal'.");
    }

    private static async Task<IResult> DegradeUserToUsuarioNormal(
        string id,
        [FromServices] IMediator mediator)
    {
        var command = new DegradeUserToUsuarioNormalCommand(id);
        var result = await mediator.Send<DegradeUserToUsuarioNormalCommand, ApplicationUser>(command);
        
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<ApplicationUser>.CreateSuccess(
                result.Value, 
                "Usuario degradado exitosamente a UsuarioNormal"
            ));
        }
        
        return result.ToHttpResult();
    }


    private static async Task<IResult> GetUsuariosNormal(IMediator mediator)
    {
        var query = new GetAllUsuariosNormalQuery();
        var result = await mediator.Send<GetAllUsuariosNormalQuery, IEnumerable<ApplicationUser>>(query);
        return result.ToHttpResult();
    }

    private static async  Task<IResult> GetJefeProcesos(IMediator mediator)
    {
        var query = new GetAllJefeProcesosQuery();
        var result = await mediator.Send<GetAllJefeProcesosQuery, IEnumerable<ApplicationUser>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetJefeAreas(IMediator mediator)
    {
        var query = new GetAllJefeAreasQuery();
        var result = await mediator.Send<GetAllJefeAreasQuery, IEnumerable<ApplicationUser>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetUser(string id , IMediator mediator)
    {
        var query = new GetUserQuery(id);
        var result = await mediator.Send<GetUserQuery, ApplicationUser>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetAllUsers(IMediator mediator)
    {
        var query = new GetAllUsersQuery();
        var result = await mediator.Send<GetAllUsersQuery, IEnumerable<ApplicationUser>>(query);
        return result.ToHttpResult();
    }
}