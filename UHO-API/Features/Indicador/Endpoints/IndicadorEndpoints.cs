using Microsoft.AspNetCore.Mvc;
using UHO_API.Core.Extensions;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Indicador.Commands;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Queries;
using UHO_API.Shared.Dtos;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Endpoints;


public static class IndicadorEndpoints
{
    public static void MapIndicadorEndpoints(this WebApplication app)
    {
        var grp = app.MapGroup("indicadores").WithTags("Indicadores");

        grp.MapGet("/{id:int}", GetIndicadorById).WithName("GetIndicadorById");
        grp.MapPost("/create", CreateIndicador).WithName("CreateIndicador");
        grp.MapGet("/", GetAllIndicadores).WithName("GetAllIndicadores");
        grp.MapGet("/deleted", GetDeletedIndicadores).WithName("GetDeletedIndicadores");
        grp.MapPut("/{id:int}", UpdateIndicador).WithName("UpdateIndicador");
        grp.MapDelete("/soft-delete/{id:int}", SoftDeleteIndicador).WithName("SoftDeleteIndicador");
        grp.MapDelete("/hard-delete/{id:int}", HardDeleteIndicador).WithName("HardDeleteIndicador");
    }

    private static async Task<IResult> CreateIndicador([FromBody] CreateIndicadorDto dto, [FromServices] IMediator mediator)
    {
        var command = new CreateIndicadorCommand(dto);
        var result = await mediator.Send<CreateIndicadorCommand, IndicadorDto>(command);

        return result.Match(
            onSuccess: value => Results.Created($"/indicadores/{value.Id}", ApiResponse<IndicadorDto>.CreateSuccess(value, "Indicador creado exitosamente")),
            onFailure: error => error.ToHttpResult()
        );
    }

    private static async Task<IResult> GetAllIndicadores([FromServices] IMediator mediator)
    {
        var query = new GetAllIndicadoresQuery();
        var result = await mediator.Send<GetAllIndicadoresQuery, IEnumerable<IndicadorDto>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetDeletedIndicadores([FromServices] IMediator mediator)
    {
        var query = new GetDeletedIndicadoresQuery();
        var result = await mediator.Send<GetDeletedIndicadoresQuery, IEnumerable<IndicadorDto>>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetIndicadorById(int id, [FromServices] IMediator mediator)
    {
        var query = new GetIndicadorByIdQuery(id);
        var result = await mediator.Send<GetIndicadorByIdQuery, IndicadorDto>(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateIndicador(int id, [FromBody] UpdateIndicadorDto dto, [FromServices] IMediator mediator)
    {
        var command = new UpdateIndicadorCommand(id, dto);
        var result = await mediator.Send<UpdateIndicadorCommand, IndicadorDto>(command);

        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<IndicadorDto>.CreateSuccess(result.Value, "Indicador actualizado exitosamente"));
        }

        return result.ToHttpResult();
    }

    private static async Task<IResult> SoftDeleteIndicador(int id, [FromServices] IMediator mediator)
    {
        var command = new SoftDeleteIndicadorCommand(id);
        var result = await mediator.Send<SoftDeleteIndicadorCommand, bool>(command);
        if (result.IsSuccess) return Results.NoContent();
        return result.ToHttpResult();
    }

    private static async Task<IResult> HardDeleteIndicador(int id, [FromServices] IMediator mediator)
    {
        var command = new HardDeleteIndicadorCommand(id);
        var result = await mediator.Send<HardDeleteIndicadorCommand, bool>(command);
        if (result.IsSuccess) return Results.NoContent();
        return result.ToHttpResult();
    }
}