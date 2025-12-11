using UHO_API.Core.Interfaces;
using UHO_API.Features.Indicador.Commands;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Queries;

namespace UHO_API.Extensions.ModelsExtensions;

public static  class IndicadorExtensions
{
    public static void AddIndicadorQueriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GetIndicadorByIdQuery, IndicadorDto>, GetIndicadorByIdHandler>();
        services.AddScoped<IRequestHandler<GetAllIndicadoresQuery, IEnumerable<IndicadorDto>>, GetAllIndicadoresHandler>();
    }

    public static void AddIndicadorCommandsConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateIndicadorCommand, IndicadorDto>, CreateIndicadorHandler>();
        services.AddScoped<IRequestHandler<UpdateIndicadorCommand, IndicadorDto>, UpdateIndicadorHandler>();
        services.AddScoped<IRequestHandler<HardDeleteIndicadorCommand, bool>, HardDeleteIndicadorHandler>();
    }
}