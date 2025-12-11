using UHO_API.Core.Enums;

namespace UHO_API.Features.IndicadorDeArea.Dto;

public class IndicadorDeAreaDto
{
    public int Id { get; set; }
    public int IndicadorId { get; set; }
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public string MetaCumplir { get; set; } = string.Empty;
    public string? MetaReal { get; set; }
    public EvaluationType Evaluacion { get; set; }
    public string Comentario { get; set; } = string.Empty;
}