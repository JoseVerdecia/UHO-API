using Microsoft.AspNetCore.Mvc;
using UHO_API.Core.Entities;
using UHO_API.Core.Extensions;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Features.Area.Commands;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Queries;
using UHO_API.Shared.Dtos;
using UHO_API.Shared.Results;


namespace UHO_API.Features.Area.Endpoints;

public static class AreaEndpoints
{
    
    public static void MapAreaEndpoints(this WebApplication app)
    {
        var areasGroup = app.MapGroup("areas");

        areasGroup.MapGet("/{id:int}",GetAreaById).WithName("GetAreaById");
        areasGroup.MapPost("/create-area", CreateArea).WithName("CreateArea");
        areasGroup.MapGet("/", GetAllAreas).WithName("GetAllAreas");
        areasGroup.MapGet("/all-delete-areas", GetAllSoftDeletableAreas).WithName("GetAllSoftDeletableAreas");
        areasGroup.MapGet("/soft-delete-area/{id:int}", GetSoftDeleteArea).WithName("GetSoftDeleteArea");
        areasGroup.MapPut("/{id:int}", UpdateArea).WithName("UpdateArea");
        areasGroup.MapDelete("/soft-delete/{id:int}", SoftDeleteArea).WithName("SoftDeleteArea");
        areasGroup.MapDelete("/hard-delete/{id:int}", HardDeleteArea).WithName("HardDeleteArea");
    }
  
    private static async Task<IResult> GetSoftDeleteArea(int id ,[FromServices] IMediator mediator)
    {
        var query = new GetSoftDeleteAreaQuery(id);
        var result = await mediator.Send<GetSoftDeleteAreaQuery, AreaResponse>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetAllSoftDeletableAreas([FromServices] IMediator mediator)
    {
        var query = new GetAllSoftDeleteAreasQuery();
        var result =  await mediator.Send<GetAllSoftDeleteAreasQuery, IEnumerable<AreaResponse>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> HardDeleteArea(int id , IMediator mediator)
    {
        var command = new HardDeleteCommand(id);
        var result = await mediator.Send<HardDeleteCommand, bool>(command);
        
        if (result.IsSuccess)
            return Results.NoContent();

        return result.ToHttpResult();
    }

    private static async Task<IResult> SoftDeleteArea(int id, IMediator mediator)
    {
        var command = new SoftDeleteAreaCommand(id);
        var result = await mediator.Send<SoftDeleteAreaCommand, bool>(command);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateArea(int id, [FromBody] UpdateAreaCommand command,[FromServices] IMediator mediator)
    {
        var commandWithId = command with { Id = id };
        if (string.IsNullOrWhiteSpace(commandWithId.Nombre))
        {
            return Results.BadRequest(ApiResponse.CreateFailure(
                Error.Validation("Nombre", "El nombre del área es requerido")
            ));
        }
    
        var result = await mediator.Send<UpdateAreaCommand, AreaResponse>(commandWithId);
    
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<AreaResponse>.CreateSuccess(
                result.Value, 
                "Área actualizada exitosamente"
            ));
        }
    
        return result.ToHttpResult();
    }

    private static async  Task<IResult> GetAllAreas(IMediator mediator)
    {
        var query = new GetAllAreaQuery();
        var result =  await mediator.Send<GetAllAreaQuery, IEnumerable<AreaResponse>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateArea(
        [FromBody] CreateAreaCommand command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send<CreateAreaCommand, AreaResponse>(command);
        
        return result.Match(
            onSuccess: value => Results.Created(
                $"/areas/{value.Id}", 
                ApiResponse<AreaResponse>.CreateSuccess(value, "Área creada exitosamente")
            ),
            onFailure: error => error.ToHttpResult()
        );
    }

    private static async Task<IResult> GetAreaById(int id, IMediator mediator)
    {
        var query = new GetAreaByIdQuery(id);
        var result = await mediator.Send<GetAreaByIdQuery, AreaResponse>(query);
        return result.ToHttpResult();
    }
    
    
}