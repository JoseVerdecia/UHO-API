using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;

namespace UHO_API.Core.Entities;

public class ObjetivoModel: IEntity,ISoftDeletable
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El Nombre del Objetivo es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    
    public List<IndicadorModel> Indicadores { get; set; } = new();
    
    public EvaluationType Evaluacion { get; set; } = EvaluationType.NoEvaluado;

    [NotMapped]
    public List<ProcesoModel> ProcesosInferidos
    {
        get
        {
            return Indicadores?
                .Select(i => i.Proceso)
                .Where(p => p != null)
                .DistinctBy(p => p.Id) 
                .ToList() ?? new List<ProcesoModel>();
        }
    }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}