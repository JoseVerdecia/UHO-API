using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
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
        var userGroup = app.MapGroup("users").WithTags("Usuarios");

        userGroup.MapGet("/", GetAllUsers)
            .WithDescription("Obtiene todos los usuarios");
        
        userGroup.MapGet("/{id:Guid}", GetUser)
            .WithDescription("Obtiene un usuario por su ID");
        
        userGroup.MapGet("/users-in-rol-jefeArea", GetJefeAreas)
            .WithDescription("Obtiene los usuarios con el rol: 'JefeArea'");
        
        userGroup.MapGet("/users-in-rol-jefeProceso", GetJefeProcesos)
            .WithDescription("Obtiene los usuarios con el rol: 'JefeProceso'");
        
        userGroup.MapGet("/users-in-rol-usuario-normal", GetUsuariosNormal)
            .WithDescription("Obtiene los usuarios con el rol: 'UsuarioNormal'");

        userGroup.MapPost("/degrade-user/{id:Guid}", DegradeUserToUsuarioNormal)
            .WithName("DegradeUser")
            .WithDescription("Quita los roles de 'JefeArea' o 'JefeProceso' a un usuario y le asigna el rol 'UsuarioNormal'.");
    }

    private static async Task<IResult> DegradeUserToUsuarioNormal(string id,[FromServices] IMediator mediator)
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