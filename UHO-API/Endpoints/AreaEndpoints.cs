using System.Collections;
using Microsoft.AspNetCore.Mvc;
using UHO_API.Extensions;
using UHO_API.Features.Area.Command;
using UHO_API.Features.Area.Dto;
using UHO_API.Features.Area.Query;
using UHO_API.Interfaces;
using UHO_API.Utilities;

namespace UHO_API.Endpoints;

public static class AreaEndpoints
{
    public static void MapAreaEndpoints(this WebApplication app)
    {
        var areasGroup = app.MapGroup("areas");

        areasGroup.MapGet("/{id:int}",GetAreaById).WithName("GetAreaById");
        areasGroup.MapPost("/create-area", CreateArea).WithName("CreateArea");
        areasGroup.MapGet("/", GetAllAreas).WithName("GetAllAreas");
        areasGroup.MapPut("/{id:int}", UpdateArea).WithName("UpdateArea");
        areasGroup.MapDelete("/soft-delete/{id:int}", SoftDeleteArea).WithName("SoftDeleteArea");
        areasGroup.MapDelete("/hard-delete/{id:int}", HardDeleteArea).WithName("HardDeleteArea");
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

    private static async Task<IResult> UpdateArea(int id, [FromBody] UpdateAreaCommand command, IMediator mediator)
    {
        var commandWithId = command with { Id = id };
        var result = await mediator.Send<UpdateAreaCommand, AreaResponse>(commandWithId);
        return result.ToHttpResult();
    }

    private static async  Task<IResult> GetAllAreas(IMediator mediator)
    {
        var query = new GetAllAreaQuery();
        var result =  await mediator.Send<GetAllAreaQuery, IEnumerable<AreaResponse>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateArea([FromBody] CreateAreaCommand command, IMediator mediator)
    {
        var result = await mediator.Send<CreateAreaCommand, AreaResponse>(command);

        if (result.IsSuccess)
        {
            return Results.CreatedAtRoute("GetAreaById", new { id = result.Value.Id }, result.Value);
        }

        return result.ToHttpResult();
    }

    private static async Task<IResult> GetAreaById(int id, IMediator mediator)
    {
        var query = new GetAreaByIdQuery(id);
        var result = await mediator.Send<GetAreaByIdQuery, AreaResponse>(query);
        return result.ToHttpResult();
    }
}