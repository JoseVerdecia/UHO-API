using UHO_API.Core.Entities;
using UHO_API.Features.Area.Dtos;
using UHO_API.Features.Proceso.Dtos;

namespace UHO_API.Features.Proceso.Mappings;

public static class ProcesoMappings
{
    public static IEnumerable<ProcesoDto> MapToProcesosDto(this IEnumerable<ProcesoModel> procesos)
    {
        return procesos.Select(proceso => new ProcesoDto
        {
            Id = proceso.Id,
            Nombre = proceso.Nombre,
            JefeProcesoId = proceso.JefeDeProcesoId,
            JefeProcesoNombre = proceso.JefeDeProceso?.FullName ?? "No Asigando",
            Email = proceso.JefeDeProceso?.Email ?? "N/A",

        });
    }

    public static ProcesoDto MapToDto(this ProcesoModel proceso)
    {
        return new ProcesoDto
        {
            Id = proceso.Id,
            Nombre = proceso.Nombre,
            JefeProcesoId = proceso.JefeDeProcesoId,
            JefeProcesoNombre = proceso.JefeDeProceso?.FullName ?? "No Asigando",
            Email = proceso.JefeDeProceso?.Email ?? "N/A"
        };
    }

}
