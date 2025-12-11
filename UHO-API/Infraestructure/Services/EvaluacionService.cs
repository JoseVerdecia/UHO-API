using System.Globalization;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Extensions;
using UHO_API.Shared.Results;

namespace UHO_API.Infraestructure.Services;


public class EvaluacionService<T> : IEvaluacionService<T> where T : IEvaluable
{
    

    public Result SetMetaCumplir(T entity, string metaCumplir)
    {
        try
        {
            var (valor, esPorcentaje, original) = ParsearMeta(metaCumplir, nameof(entity.MetaCumplir));
            entity.MetaCumplir = original;
            entity.DecimalMetaCumplir = valor;
            entity.IsMetaCumplirPorcentage = esPorcentaje;
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation(nameof(entity.MetaCumplir), ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure("ParseMetaCumplir", ex.Message));
        }
    }

    public Result SetMetaReal(T entity, string? metaReal)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(metaReal))
            {
                entity.MetaReal = string.Empty;
                entity.DecimalMetaReal = 0;
                entity.IsMetaRealPorcentage = false;

                
                entity.Evaluacion = EvaluationType.NoEvaluado;
                return Result.Success();
            }

            var (valor, esPorcentaje, original) = ParsearMeta(metaReal, nameof(entity.MetaReal));
            entity.MetaReal = original;
            entity.DecimalMetaReal = valor;
            entity.IsMetaRealPorcentage = esPorcentaje;

            // Recalcular evaluación
            entity.Evaluacion = CalcularEvaluacion(entity.DecimalMetaCumplir, entity.DecimalMetaReal);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation(nameof(entity.MetaReal), ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure("ParseMetaReal", ex.Message));
        }
    }

    public Result Evaluate(T entity)
    {
        // Si metaCumplir = 0 -> no evaluable
        if (entity.DecimalMetaCumplir == 0)
        {
            entity.Evaluacion = EvaluationType.NoEvaluado;
            return Result.Success();
        }

        if (string.IsNullOrWhiteSpace(entity.MetaReal) || entity.DecimalMetaReal == 0)
        {
            entity.Evaluacion = EvaluationType.Incumplido;
            return Result.Success();
        }

        entity.Evaluacion = CalcularEvaluacion(entity.DecimalMetaCumplir, entity.DecimalMetaReal);
        return Result.Success();
    }

    public Result EvaluateAll(IEnumerable<T> entities)
    {
        foreach (var e in entities)
        {
            var r = Evaluate(e);
            if (r.IsFailure) return r;
        }
        return Result.Success();
    }

    public string GetMetaCumplirDisplay(T entity) => entity.IsMetaCumplirPorcentage ? $"{entity.DecimalMetaCumplir}%" : entity.DecimalMetaCumplir.ToString(CultureInfo.InvariantCulture);
    public string GetMetaRealDisplay(T entity) => entity.IsMetaRealPorcentage ? $"{entity.DecimalMetaReal}%" : entity.DecimalMetaReal.ToString(CultureInfo.InvariantCulture);
    public EvaluationType GetEvaluacion(T entity) => entity.Evaluacion;
    public string GetEvaluacionDisplay(T entity) => entity.Evaluacion.GetDisplayName();

    // --- Helpers privados ---
    
    /// <summary>
    /// Parsea y valida una cadena de meta numérica o porcentual.
    /// Convierte el formato de cadena a valores numéricos y banderas de porcentaje.
    /// </summary>
    /// <param name="meta">Valor de la meta en formato cadena</param>
    /// <param name="nombreCampo">Nombre del campo para mensajes de error</param>
    /// <returns>Tupla con: valor decimal, bandera de porcentaje, valor original</returns>
    /// <exception cref="ArgumentException">Cuando el formato de la meta es inválido</exception>
    private static (decimal valor, bool esPorcentaje, string valorOriginal) ParsearMeta(string meta, string nombreCampo)
    {
        if (string.IsNullOrWhiteSpace(meta))
            throw new ArgumentException($"{nombreCampo} no puede estar vacío.");

        string valorOriginal = meta.Trim();
        bool esPorcentaje = false;
        string valorLimpio = valorOriginal;

        if (valorLimpio.EndsWith("%"))
        {
            if (valorLimpio.Count(c => c == '%') > 1)
                throw new ArgumentException($"{nombreCampo}: Formato de porcentaje inválido. Múltiples '%'.");
            esPorcentaje = true;
            valorLimpio = valorLimpio[..^1].Trim();
        }

        if (valorLimpio.StartsWith('.') || valorLimpio.EndsWith('.'))
            throw new ArgumentException($"{nombreCampo}: Formato decimal inválido ('{valorOriginal}').");

        if (!decimal.TryParse(valorLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out var valorDecimal))
            throw new ArgumentException($"{nombreCampo}: '{meta}' no es un decimal válido.");

        return (valorDecimal, esPorcentaje, valorOriginal);
    }

    
    /// <summary>
    /// Calcula el tipo de evaluación basado en los valores de meta de cumplimiento y meta real.    
    /// </summary>
    /// <param name="metaCumplir">Valor de la meta de cumplimiento</param>
    /// <param name="metaReal">Valor de la meta real alcanzada</param>
    /// <returns>Tipo de evaluación calculado</returns>
    /// <remarks>
    /// - Si metaCumplir = 0: No Evaluado
    /// - Si metaReal = 0 y metaCumplir != 0: Incumplido (cumple requisito)
    /// - Porcentaje de cumplimiento > 100: Sobrecumplido
    /// - Porcentaje = 100: Cumplido
    /// - Porcentaje entre 80-99: Parcialmente Cumplido
    /// - Porcentaje < 80 : Incumplido
    /// </remarks>
    private static EvaluationType CalcularEvaluacion(decimal metaCumplir, decimal metaReal)
    {
        if (metaCumplir == 0)
            return EvaluationType.NoEvaluado;

        // Si metaReal == 0 y metaCumplir != 0 => Incumplido    
        if (metaReal == 0)
            return EvaluationType.Incumplido;

        var porcentajeCumplimiento = (metaReal / metaCumplir) * 100m;
        if (porcentajeCumplimiento > 100) return EvaluationType.Sobrecumplido;
        if (porcentajeCumplimiento == 100) return EvaluationType.Cumplido;
        if (porcentajeCumplimiento >= 80) return EvaluationType.ParcialmenteCumplido;
        return EvaluationType.Incumplido;
    }
}