using UHO_API.Core.Entities;
using UHO_API.Features.Area.Commands;
using UHO_API.Features.Area.Dtos;
namespace UHO_API.Features.Area.Mappings;

public static class AreaMappings
{
    public static IEnumerable<AreaResponse> MapToAreaResponses(this IEnumerable<AreaModel> areas)
    {
        return areas.Select(area => new AreaResponse
            (
                area.Id,
                area.Nombre, 
                area.JefeAreaId, 
                area.JefeArea?.FullName ?? "No Asignado",
                area.JefeArea?.Email ?? "N/A"
            
            ));

    }

    public static AreaResponse MapToAreaResponse(this AreaModel area)
    {
        return new AreaResponse(
            area.Id,
            area.Nombre,
            area.JefeAreaId,
            area.JefeArea?.FullName ?? "No asignado",
            area.JefeArea?.Email ?? "N/A"
        );
    }

    public static AreaModel MapToAreaModel(this CreateAreaCommand command)
    {
        return new AreaModel
        {
            Nombre = command.Nombre,
            JefeAreaId = command.JefeAreaId
        };
    }
}