using UHO_API.Core.Interfaces;
using UHO_API.Features.Proceso.Commands;
using UHO_API.Features.Proceso.Dtos;
using UHO_API.Features.Proceso.Queries;

namespace UHO_API.Extensions.ModelsExtensions;

public static class ProcesoExtensions
{
    public static void AddProcesoQueriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GetAllProcesoQuery, IEnumerable<ProcesoDto>>, GetAllProcesoHandler>();
        services.AddScoped<IRequestHandler<GetSoftDeleteProcesoQuery, ProcesoDto>, GetSoftDeletePrcesoHandler>();
        services.AddScoped<IRequestHandler<GetAllSoftDeleteProcesosQuery, IEnumerable<ProcesoDto>>, GetAllSoftDeleteProcesosQueryHandler>();
        services.AddScoped<IRequestHandler<GetProcesoByIdQuery, ProcesoDto>, GetProcesoByIdHandler>();
    }
    public static void AddProcesoCommandsConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateProcesoCommand, ProcesoDto>, CreateProcesoHandler>();
        services.AddScoped<IRequestHandler<UpdateProcesoCommand, ProcesoDto>, UpdateProcesoHandler>();
        services.AddScoped<IRequestHandler<SoftDeleteProcesoCommand, bool>, SoftDeleteProcesoHandler>();
        services.AddScoped<IRequestHandler<HardDeleteProcesoCommand, bool>, HardDeleteProcesoHandler>();
    }
}