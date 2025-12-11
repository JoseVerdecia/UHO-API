using UHO_API.Core.Enums;
using UHO_API.Features.IndicadorDeArea.Dto;
using UHO_API.Features.Objetivo.Dto;

namespace UHO_API.Features.Indicador.Dtos;

public class IndicadorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    public string MetaCumplir { get; set; }
    public string? MetaReal { get; set; }

    public decimal DecimalMetaCumplir { get; set; }
    public decimal DecimalMetaReal { get; set; }

    public bool IsMetaCumplirPorcentage { get; set; }
    public bool IsMetaRealPorcentage { get; set; }

    public EvaluationType Evaluacion { get; set; }

    public int ProcesoId { get; set; }
    public string? ProcesoNombre { get; set; }

    public List<ObjetivoDto> Objetivos { get; set; } = new();
}
