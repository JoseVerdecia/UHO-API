using UHO_API.Models;

namespace UHO_API.Features.Area.Dto;

public record AreaResponse(
     int Id,
     string Nombre,
     string JefeAreaId,
     string JefeAreaNombre,
     string JefeAreaEmail
);