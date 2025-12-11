namespace UHO_API.Features.Indicador.Dtos;

public class UpdateIndicadorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string MetaCumplir { get; set; } = string.Empty;
    public string? MetaReal { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public int ProcesoId { get; set; }
    public List<int> ObjetivoIds { get; set; } = new();
}