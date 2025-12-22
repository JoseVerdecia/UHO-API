using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class IndicadorDeAreaModel :IEntity,ISoftDeletable,IEvaluable
{
    [Key]   
    public int Id { get; set; }
    
    // Clave Compuesta
    public int IndicadorId { get; set; }
    public int AreaId { get; set; }
    
    // Propiedades
    
    [Required]
    public string MetaCumplir { get; set; } = string.Empty;
    public decimal DecimalMetaCumplir { get; set; } = 0;
    public bool IsMetaCumplirPorcentage { get; set; } = false;
    public string MetaReal { get; set; } = string.Empty;
    public decimal DecimalMetaReal { get; set; } = 0;
    public bool IsMetaRealPorcentage { get; set; } = false;
    
    public EvaluationType Evaluacion { get; set; } = EvaluationType.NoEvaluado;

    // Relaciones
    [ForeignKey("IndicadorId")]
    public IndicadorModel Indicador { get; set; } = null!;

    [ForeignKey("AreaId")]
    public AreaModel Area { get; set; } = null!;
    
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
}