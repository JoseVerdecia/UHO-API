using UHO_API.Core.Interfaces;
using UHO_API.Features.Area.Commands;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Area.Queries;

namespace UHO_API.Extensions.ModelsExtensions;

public static class AreaExtensions
{
    public static void AddAreaQueriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<GetAllAreaQuery, IEnumerable<AreaResponse>>, GetAllAreaHandler>(); 
        services.AddScoped<IRequestHandler<GetAllSoftDeleteAreasQuery, IEnumerable<AreaResponse>>, GetAllSoftDeleteAreasHandler>();
        services.AddScoped<IRequestHandler<GetSoftDeleteAreaQuery, AreaResponse>, GetSoftDeleteAreaHandler>();
        services.AddScoped<IRequestHandler<GetAreaByIdQuery, AreaResponse>, GetAreaByIdHandler>();

    }

    public static void AddAreaCommandsConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateAreaCommand, AreaResponse>, CreateAreaHandler>();
        services.AddScoped<IRequestHandler<UpdateAreaCommand, AreaResponse>, UpdateAreaHandler>();
        services.AddScoped<IRequestHandler<SoftDeleteAreaCommand, bool>, SoftDeleteAreaHandler>();
        services.AddScoped<IRequestHandler<HardDeleteCommand, bool>, HardDeleteHandler>();
    }
}