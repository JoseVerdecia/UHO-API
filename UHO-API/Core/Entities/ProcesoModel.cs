using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class ProcesoModel: IEntity,ISoftDeletable
{
    [Key]
    public int Id { get; set; }
    
    public string Nombre { get; set; } = string.Empty;

    public IEnumerable<IndicadorModel> Indicadores { get; set; } = new List<IndicadorModel>();
    
    public EvaluationType Evaluacion { get; set; } = EvaluationType.NoEvaluado;
    
    // Responsable
    public string? JefeDeProcesoId { get; set; }
    
    [ForeignKey("JefeDeProcesoId")]
    public ApplicationUser? JefeDeProceso { get; set; }
    
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}