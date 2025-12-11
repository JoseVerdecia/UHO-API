using System.ComponentModel.DataAnnotations;

namespace UHO_API.Core.Enums;


public enum EvaluationType
{
    [Display(Name = "Sobrecumplido")]
    Sobrecumplido,
    [Display(Name = "Cumplido")]
    Cumplido,
    [Display(Name = "Parcialmente cumpldio")]
    ParcialmenteCumplido,
    [Display(Name = "Incumplido")]
    Incumplido,
    [Display(Name = "No evaluado")]
    NoEvaluado
}