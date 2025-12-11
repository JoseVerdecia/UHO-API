using UHO_API.Core.Entities;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Mappings;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Commands;

public record UpdateIndicadorCommand(int Id, UpdateIndicadorDto Dto) : IRequest<IndicadorDto>;

public class UpdateIndicadorHandler : IRequestHandler<UpdateIndicadorCommand, IndicadorDto>
{
    private readonly IUnitOfWorks _uow;
    private readonly IEvaluacionService<IndicadorModel> _evaluacionService;
    private readonly ILogger<UpdateIndicadorHandler> _logger;

    public UpdateIndicadorHandler(IUnitOfWorks uow, IEvaluacionService<IndicadorModel> evaluacionService, ILogger<UpdateIndicadorHandler> logger)
    {
        _uow = uow;
        _evaluacionService = evaluacionService;
        _logger = logger;
    }

    public async Task<Result<IndicadorDto>> Handle(UpdateIndicadorCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var dto = request.Dto;

        var indicador = await _uow.Indicador.Get(i => i.Id == id, includeProperties: "Proceso,Objetivos,IndicadoresAsignados");
        if (indicador is null) return Result.Failure<IndicadorDto>(Error.NotFound("Indicador", id.ToString()));

        // Validaciones
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return Result.Failure<IndicadorDto>(Error.Validation("Nombre", "El nombre del indicador es requerido"));

        if (string.IsNullOrWhiteSpace(dto.MetaCumplir))
            return Result.Failure<IndicadorDto>(Error.Validation("MetaCumplir", "La meta a cumplir es requerida"));

        // Proceso
        var proceso = await _uow.Proceso.Get(p => p.Id == dto.ProcesoId);
        if (proceso is null) return Result.Failure<IndicadorDto>(Error.NotFound("Proceso", dto.ProcesoId.ToString()));

        // Objetivos
        var objetivos = new List<ObjetivoModel>();
        foreach (var objetivoId in dto.ObjetivoIds.Distinct())
        {
            var obj = await _uow.Objetivo.Get(o => o.Id == objetivoId);
            if (obj is null) return Result.Failure<IndicadorDto>(Error.NotFound("Objetivo", objetivoId.ToString()));
            objetivos.Add(obj);
        }

        using var tx = await _uow.BeginTransactionAsync();
        try
        {
            indicador.Nombre = dto.Nombre.Trim();
            indicador.Comentario = dto.Comentario?.Trim() ?? string.Empty;
            indicador.ProcesoId = dto.ProcesoId;
            indicador.Proceso = proceso;
            indicador.Objetivos = objetivos;
            indicador.UpdatedAt = DateTime.UtcNow;

            // Setear metaCumplir
            var r1 = _evaluacionService.SetMetaCumplir(indicador, dto.MetaCumplir);
            if (r1.IsFailure) return Result.Failure<IndicadorDto>(r1.Error);

            // Setear metaReal (puede ser null/empty)
            var r2 = _evaluacionService.SetMetaReal(indicador, dto.MetaReal);
            if (r2.IsFailure) return Result.Failure<IndicadorDto>(r2.Error);

            _uow.Indicador.Update(indicador);
            await _uow.SaveChangesAsync();
            await tx.CommitAsync();

            var response = indicador.ToDto();
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger.LogError(ex, "Error actualizando indicador {Id}", id);
            return Result.Failure<IndicadorDto>(Error.Failure("UpdateFailed", $"Error al actualizar el indicador: {ex.Message}"));
        }
    }
}