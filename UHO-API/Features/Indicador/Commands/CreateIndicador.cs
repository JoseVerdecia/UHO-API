using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.Indicador.Mappings;
using UHO_API.Features.IndicadorDeArea.Dto;
using UHO_API.Features.Objetivo.Dto;
using UHO_API.Shared.Results;

namespace UHO_API.Features.Indicador.Commands;


public record CreateIndicadorCommand(CreateIndicadorDto Dto) : IRequest<IndicadorDto>;

public class CreateIndicadorHandler : IRequestHandler<CreateIndicadorCommand, IndicadorDto>
{
    private readonly IUnitOfWorks _uow;
    private readonly IEvaluacionService<IndicadorModel> _evaluacionService;
    private readonly ILogger<CreateIndicadorHandler> _logger;

    public CreateIndicadorHandler(IUnitOfWorks uow, IEvaluacionService<IndicadorModel> evaluacionService,
        ILogger<CreateIndicadorHandler> logger)
    {
        _uow = uow;
        _evaluacionService = evaluacionService;
        _logger = logger;
    }


    public async Task<Result<IndicadorDto>> Handle(CreateIndicadorCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return Result.Failure<IndicadorDto>(Error.Validation("Nombre", "El nombre del indicador es requerido"));

        if (string.IsNullOrWhiteSpace(dto.MetaCumplir))
            return Result.Failure<IndicadorDto>(Error.Validation("MetaCumplir", "La meta a cumplir es requerida"));

      
        var proceso = await _uow.Proceso.Get(p => p.Id == dto.ProcesoId);
        if (proceso is null)
            return Result.Failure<IndicadorDto>(Error.NotFound("Proceso", dto.ProcesoId.ToString()));

       
        var objetivos = new List<ObjetivoModel>();
        foreach (var id in dto.ObjetivoIds.Distinct())
        {
            var objetivo = await _uow.Objetivo.Get(o => o.Id == id);
            if (objetivo is null)
                return Result.Failure<IndicadorDto>(Error.NotFound("Objetivo", id.ToString()));
            objetivos.Add(objetivo);
        }

        var now = DateTime.UtcNow;
        
        var indicador = new IndicadorModel
        {
            Nombre = dto.Nombre.Trim(),
            MetaCumplir = string.Empty, 
            MetaReal = string.Empty,
            Comentario = dto.Comentario?.Trim() ?? string.Empty,
            ProcesoId = dto.ProcesoId,
            Proceso = proceso,
            Objetivos = objetivos,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

     
        var r1 = _evaluacionService.SetMetaCumplir(indicador, dto.MetaCumplir);
        if (r1.IsFailure) return Result.Failure<IndicadorDto>(r1.Error);

     
        if (!string.IsNullOrWhiteSpace(dto.MetaReal))
        {
            var r2 = _evaluacionService.SetMetaReal(indicador, dto.MetaReal);
            if (r2.IsFailure) return Result.Failure<IndicadorDto>(r2.Error);
        }
        else
        {
            indicador.MetaReal = string.Empty;
            indicador.DecimalMetaReal = 0;
            indicador.IsMetaRealPorcentage = false;
            indicador.Evaluacion = EvaluationType.NoEvaluado;
        }

        try
        {
            _uow.Indicador.Add(indicador);
            await _uow.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando Indicador {Nombre}", dto.Nombre);
            return Result.Failure<IndicadorDto>(Error.Failure("DatabaseError", $"Error al guardar el indicador: {ex.Message}"));
        }

        var response = indicador.ToDto();
        return Result.Success(response);
    }
}