using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Commands;

public record HardDeleteIndicadorCommand(int Id) : IRequest<bool>;

public class HardDeleteIndicadorHandler : IRequestHandler<HardDeleteIndicadorCommand, bool>
{
    private readonly IUnitOfWorks _uow;
    private readonly ILogger<HardDeleteIndicadorHandler> _logger;

    public HardDeleteIndicadorHandler(IUnitOfWorks uow, ILogger<HardDeleteIndicadorHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(HardDeleteIndicadorCommand request, CancellationToken cancellationToken)
    {
        var indicador = await _uow.Indicador.GetById(request.Id);
        if (indicador is null) return Result.Failure<bool>(Error.NotFound("Indicador", request.Id.ToString()));

        try
        {
            _uow.Indicador.Delete(indicador);
            await _uow.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al hard delete Indicador {Id}", request.Id);
            return Result.Failure<bool>(Error.Failure("DeleteFailed", $"Error al eliminar el indicador: {ex.Message}"));
        }
    }
}