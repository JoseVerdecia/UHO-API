using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Commands;

public record SoftDeleteIndicadorCommand(int Id) : IRequest<bool>;

public class SoftDeleteIndicadorHandler : IRequestHandler<SoftDeleteIndicadorCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly ILogger<SoftDeleteIndicadorHandler> _logger;

    public SoftDeleteIndicadorHandler(IUnitOfWorks uow, ILogger<SoftDeleteIndicadorHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(SoftDeleteIndicadorCommand request, CancellationToken cancellationToken)
    {
        var indicador = await _uow.Indicador.GetById(request.Id);
        if (indicador is null) return Result.Failure<bool>(Error.NotFound("Indicador", request.Id.ToString()));
        if (indicador.IsDeleted) return Result.Failure<bool>(Error.Business("AlreadyDeleted", "El indicador ya está eliminado"));

        try
        {
            await _uow.Indicador.SoftDelete(request.Id);
            await _uow.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al soft delete Indicador {Id}", request.Id);
            return Result.Failure<bool>(Error.Failure("SoftDeleteFailed", $"Error al marcar como eliminada el indicador: {ex.Message}"));
        }
    }
}