using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class IndicadorModel: IEntity,ISoftDeletable,IEvaluable
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El nombre del Indicador es Obligatorio")]
    [MaxLength(250, ErrorMessage = "Nombre del Indicador muy largo")]
    public string Nombre { get; set; } = string.Empty;
        
    // Propiedades
    
    [Required(ErrorMessage = "La meta a cumplir del Indicador es obligatoria")]
    public string MetaCumplir { get; set; }
    public string? MetaReal { get; set; }
    public decimal DecimalMetaCumplir { get; set; }
    public decimal DecimalMetaReal { get; set; }
    public bool IsMetaCumplirPorcentage { get; set; }
    public bool IsMetaRealPorcentage { get; set; }
    public EvaluationType Evaluacion { get; set; }
    
    public string Comentario { get; set; }
    
    public string GetEntityIdentifier() => Id.ToString();
    
    //Relaciones
    [Required(ErrorMessage = "El Indicador debe tener un Proceso asignado")]
    public int ProcesoId { get; set; }

    [ForeignKey("ProcesoId")]
    public ProcesoModel Proceso { get; set; } = null!;

    public IEnumerable<IndicadorDeAreaModel> IndicadoresAsignados { get; set; } = new List<IndicadorDeAreaModel>();
    
    [Required(ErrorMessage = "El Indicador debe tener al menos un Objetivo asignado")]
    public IEnumerable<ObjetivoModel> Objetivos { get; set; } = new List<ObjetivoModel>();
    
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}