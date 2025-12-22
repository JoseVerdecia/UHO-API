using Microsoft.AspNetCore.Mvc;
using UHO_API.Core.Extensions;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Features.Area.Commands;
using UHO_API.Features.Proceso.Commands;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Features.Proceso.Queries;
using UHO_API.Shared.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Proceso.Endpoints;

public static class ProcesoEndpoints
{
    public static void MapProcesoEndpoints(this WebApplication app)
    {
        var procesoGroup = app.MapGroup("procesos").WithTags("Procesos");
        
        procesoGroup.MapGet("/{id:int}",GetProcesoById).WithName("GetProcesoById");
        procesoGroup.MapPost("/create-proceso", CreateProceso).WithName("CreateProceso");
        procesoGroup.MapGet("/", GetAllProcesos).WithName("GetAllProcesos");
        procesoGroup.MapGet("/all-delete-procesos", GetAllSoftDeletableProcesos).WithName("GetAllSoftDeletableProcesos");
        procesoGroup.MapGet("/soft-delete-proceso/{id:int}", GetSoftDeleteProceso).WithName("GetSoftDeleteProceso");
        procesoGroup.MapPut("/{id:int}", UpdateProceso).WithName("UpdateProceso");
        procesoGroup.MapDelete("/soft-delete/{id:int}", SoftDeleteProceso).WithName("SoftDeleteProceso");
        procesoGroup.MapDelete("/hard-delete/{id:int}", HardDeleteProceso).WithName("HardDeleteProceso");
    }

    private static async Task<IResult> GetSoftDeleteProceso([FromRoute]int id ,[FromServices] IMediator mediator)
    {
        var query = new GetSoftDeleteProcesoQuery(id);
        var result = await mediator.Send<GetSoftDeleteProcesoQuery, ProcesoDto>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetAllSoftDeletableProcesos([FromServices] IMediator mediator)
    {
        var query = new GetAllSoftDeleteProcesosQuery();
        var result =  await mediator.Send<GetAllSoftDeleteProcesosQuery, IEnumerable<ProcesoDto>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> HardDeleteProceso(int id , IMediator mediator)
    {
        var command = new HardDeleteCommand(id);
        var result = await mediator.Send<HardDeleteCommand, bool>(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> SoftDeleteProceso(int id, IMediator mediator)
    {
        var command = new SoftDeleteProcesoCommand(id);
        var result = await mediator.Send<SoftDeleteProcesoCommand, bool>(command);

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateProceso(int id, [FromBody] UpdateProcesoCommand command,[FromServices] IMediator mediator)
    {
        UpdateProcesoCommand commandWithId = command with { Id = id };
        
        if (string.IsNullOrWhiteSpace(commandWithId.Nombre))
        {
            return Results.BadRequest(ApiResponse.CreateFailure(
                Error.Validation("Nombre", "El nombre del proceso es requerido")
            ));
        }
    
        var result = await mediator.Send<UpdateProcesoCommand, ProcesoDto>(commandWithId);
    
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<ProcesoDto>.CreateSuccess(
                result.Value, 
                "Proceso actualizado exitosamente"
            ));
        }
    
        return result.ToHttpResult();
    }

    private static async  Task<IResult> GetAllProcesos(IMediator mediator)
    {
        var query = new GetAllProcesoQuery();
        var result =  await mediator.Send<GetAllProcesoQuery, IEnumerable<ProcesoDto>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateProceso(
        [FromBody] CreateProcesoCommand command,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send<CreateProcesoCommand, ProcesoDto>(command);
        
        return result.Match(
            onSuccess: value => Results.Created(
                $"/procesos/{value.Id}", 
                ApiResponse<ProcesoDto>.CreateSuccess(value, "Proceso creado exitosamente")
            ),
            onFailure: error => error.ToHttpResult()
        );
    }

    private static async Task<IResult> GetProcesoById(int id, IMediator mediator)
    {
        var query = new GetProcesoByIdQuery(id);
        var result = await mediator.Send<GetProcesoByIdQuery, ProcesoDto>(query);
        return result.ToHttpResult();
    }
}