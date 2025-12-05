

namespace UHO_API.Features.Area.Dtos;

public record AreaResponse(
     int Id,
     string Nombre,
     string JefeAreaId,
     string JefeAreaNombre,
     string JefeAreaEmail
);