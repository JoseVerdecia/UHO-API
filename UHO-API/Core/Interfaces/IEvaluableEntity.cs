using UHO_API.Core.Enums;

namespace UHO_API.Core.Interfaces;

public interface IEvaluable
{
 
    string MetaCumplir { get; set; }
    decimal DecimalMetaCumplir { get; set; }
    bool IsMetaCumplirPorcentage { get; set; }

    
    string MetaReal { get; set; }
    decimal DecimalMetaReal { get; set; }
    bool IsMetaRealPorcentage { get; set; }

   
    EvaluationType Evaluacion { get; set; }
}