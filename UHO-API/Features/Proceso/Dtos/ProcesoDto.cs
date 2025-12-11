namespace UHO_API.Features.Proceso.Dtos;

public class ProcesoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string? JefeProcesoId { get; set; }
    public string JefeProcesoNombre { get; set; }
    public string Email { get; set; }
}