using UHO_API.Shared.Results;

namespace UHO_API.Core.Interfaces;

public interface IRoleChangesService
{
    /// <summary>
    /// Promueve un usuario a Jefe de Área.
    /// Si ya es Jefe de Proceso, se le quita ese rol.
    /// </summary>
    /// <param name="userId">El ID del usuario a promover.</param>
    /// <returns>True si la operación fue exitosa, false en caso contrario.</returns>
    Task<Result> PromoteToJefeAreaAsync(string userId);

    /// <summary>
    /// Promueve un usuario a Jefe de Proceso.
    /// Si ya es Jefe de Área, se le quita ese rol.
    /// </summary>
    /// <param name="userId">El ID del usuario a promover.</param>
    /// <returns>True si la operación fue exitosa, false en caso contrario.</returns>
    Task<Result> PromoteToJefeProcesoAsync(string userId);

    /// <summary>
    /// Degrada a un usuario (Jefe de Área o Jefe de Proceso) a Usuario Normal.
    /// </summary>
    /// <param name="userId">El ID del usuario a degradar.</param>
    /// <returns>True si la operación fue exitosa, false en caso contrario.</returns>
    Task<Result> DemoteToUsuarioNormalAsync(string userId);
}