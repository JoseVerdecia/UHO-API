using UHO_API.Core.Enums;
using UHO_API.Shared.Results;

namespace UHO_API.Core.Interfaces;

/// <summary>
/// Define el contrato para servicios de evaluación que gestionan el ciclo de vida de la evaluación
/// de entidades evaluables, incluyendo la configuración de metas, cálculo automático de resultados
/// y presentación de información formateada.
/// </summary>
/// <typeparam name="T">Tipo de entidad que implementa la interfaz IEvaluable y contiene propiedades de evaluación</typeparam>
public interface IEvaluacionService<T> where T : IEvaluable
{
    /// <summary>
    /// Establece la meta a cumplir para una entidad (IEvaluacion).
    /// Convierte un valor de cadena en formato numérico decimal y configura la bandera de porcentaje
    /// según el formato proporcionado (ej: "50" o "50%").
    /// </summary>
    /// <param name="entity">Entidad a la cual se le establecerá la meta de cumplimiento</param>
    /// <param name="metaCumplir">Valor de la meta en formato cadena. Puede ser número entero, decimal o porcentaje</param>
    /// <returns>Resultado de la operación con estado de éxito o error</returns>
    /// <remarks>
    /// - Valida el formato del valor proporcionado
    /// - Convierte el string a decimal y establece la bandera de porcentaje
    /// - Actualiza las propiedades internas de la entidad: MetaCumplir, DecimalMetaCumplir, IsMetaCumplirPorcentage
    /// - Devuelve Result.Success() en caso de éxito
    /// - Devuelve Result.Failure con detalles del error en caso de fallo
    /// </remarks>
    Result SetMetaCumplir(T entity, string metaCumplir);

    /// <summary>
    /// Establece la meta real alcanzada para una entidad evaluada.
    /// Al establecer este valor, se dispara automáticamente el cálculo de la evaluación
    /// basado en la meta de cumplimiento previamente configurada.
    /// </summary>
    /// <param name="entity">Entidad a la cual se le establecerá la meta real</param>
    /// <param name="metaReal">Valor de la meta real en formato cadena. Puede ser null o vacío para indicar no evaluado</param>
    /// <returns>Resultado de la operación con estado de éxito o error</returns>
    /// <remarks>
    /// - Si metaReal es null o vacío, la entidad se marca como No Evaluada,
    ///    no se considera Incumplida mientras no exista un valor real numérico.
    /// - Realiza validación y conversión similar a SetMetaCumplir
    /// - Calcula automáticamente el tipo de evaluación (Cumplido, Parcial, Incumplido, etc.)
    /// - Actualiza todas las propiedades relacionadas: MetaReal, DecimalMetaReal, IsMetaRealPorcentage, Evaluacion
    /// </remarks>
    Result SetMetaReal(T entity, string? metaReal);

    /// <summary>
    /// Realiza el cálculo de la evaluación sin modificar los valores de cadena originales.
    /// Útil para endpoints que necesitan re-evaluar una entidad existente sin alterar sus metadatos.
    /// </summary>
    /// <param name="entity">Entidad a evaluar</param>
    /// <returns>Resultado de la operación con el tipo de evaluación calculado</returns>
    /// <remarks>
    /// - Utiliza los valores decimales ya existentes en la entidad
    /// - No modifica las propiedades de cadena (MetaCumplir, MetaReal)
    /// - Ideal para consultas de solo lectura o re-evaluación
    /// - Devuelve Result.Success() siempre, ya que solo actualiza el estado interno
    /// </remarks>
    Result Evaluate(T entity);

    /// <summary>
    /// Evalúa una colección de entidades de manera batch.
    /// Procesa múltiples entidades de forma eficiente, ideal para operaciones masivas.
    /// </summary>
    /// <param name="entities">Colección de entidades a evaluar</param>
    /// <returns>Resultado de la operación con el estado de evaluación para cada entidad</returns>
    /// <remarks>
    /// - Procesa todas las entidades en la colección
    /// - Detiene el procesamiento si encuentra un error en alguna entidad
    /// - Devuelve Result.Success() solo si todas las evaluaciones son exitosas
    /// - Optimizado para operaciones en lote con early return en caso de error
    /// </remarks>
    Result EvaluateAll(IEnumerable<T> entities);

    /// <summary>
    /// Obtiene la representación formateada de la meta de cumplimiento para presentación.
    /// Muestra el valor con el formato adecuado (con o sin símbolo de porcentaje).
    /// </summary>
    /// <param name="entity">Entidad de la cual se obtendrá la meta de cumplimiento</param>
    /// <returns>Cadena formateada lista para mostrar en UI</returns>
    string GetMetaCumplirDisplay(T entity);

    /// <summary>
    /// Obtiene la representación formateada de la meta real alcanzada para presentación.
    /// Muestra el valor con el formato adecuado (con o sin símbolo de porcentaje).
    /// </summary>
    /// <param name="entity">Entidad de la cual se obtendrá la meta real</param>
    /// <returns>Cadena formateada lista para mostrar en UI</returns>
    string GetMetaRealDisplay(T entity);

    /// <summary>
    /// Obtiene el tipo de evaluación actual de la entidad.
    /// Representa el estado de cumplimiento (Cumplido, Parcial, Incumplido, etc.).
    /// </summary>
    /// <param name="entity">Entidad de la cual se obtendrá el tipo de evaluación</param>
    /// <returns>Tipo de evaluación actual</returns>
    EvaluationType GetEvaluacion(T entity);

    /// <summary>
    /// Obtiene la representación legible del tipo de evaluación para presentación en UI.
    /// Convierte el enum de evaluación a una cadena descriptiva amigable.
    /// </summary>
    /// <param name="entity">Entidad de la cual se obtendrá la descripción de evaluación</param>
    /// <returns>Cadena descriptiva del estado de evaluación</returns>
    string GetEvaluacionDisplay(T entity);
}