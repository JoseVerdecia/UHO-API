using System.ComponentModel.DataAnnotations;

namespace UHO_API.Core.Enums;

public enum AreaType
{
    [Display(Name = "Facultad")]
    Facultad,
    [Display(Name = "Municipio")]
    Municipio
}