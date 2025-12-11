using System.ComponentModel.DataAnnotations;

namespace UHO_API.Core.Enums;

public enum IndicadorOrigen
{
    [Display(Name = "Interno")]
    Interno,
    [Display(Name = "MES")]
    MES
}