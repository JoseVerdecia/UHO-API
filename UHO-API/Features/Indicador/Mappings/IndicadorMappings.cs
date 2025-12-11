using UHO_API.Core.Entities;
using UHO_API.Features.Indicador.Dtos;
using UHO_API.Features.IndicadorDeArea.Dto;
using UHO_API.Features.Objetivo.Dto;

namespace UHO_API.Features.Indicador.Mappings;

public static class IndicadorMappings
{
    public static IndicadorDto ToDto(this IndicadorModel model)
    {
        return new IndicadorDto
        {
            Id = model.Id,
            Nombre = model.Nombre,

            MetaCumplir = model.MetaCumplir,
            MetaReal = model.MetaReal,
            DecimalMetaCumplir = model.DecimalMetaCumplir,
            DecimalMetaReal = model.DecimalMetaReal,
            IsMetaCumplirPorcentage = model.IsMetaCumplirPorcentage,
            IsMetaRealPorcentage = model.IsMetaRealPorcentage,

            Evaluacion = model.Evaluacion,

            ProcesoId = model.ProcesoId,
            ProcesoNombre = model.Proceso?.Nombre,

            Objetivos = model.Objetivos
                .Select(o => new ObjetivoDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre
                })
                .ToList()
        };
    }
}