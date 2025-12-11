using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UHO_API.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// Metodo de Extension para que cada enum pueda devolver si DisplayName (Decorador)
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        if (enumValue == null)
            return string.Empty;

        var memberInfo = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        if (memberInfo == null)
        {
            return enumValue.ToString();
        }

        var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}