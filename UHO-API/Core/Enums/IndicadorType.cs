using System.ComponentModel.DataAnnotations;

namespace UHO_API.Core.Enums;

 public enum IndicadorType
    {
        [Display(Name = "Escencial")]
        Escencial,
        [Display(Name = "Necesario")]
        Necesario
    }