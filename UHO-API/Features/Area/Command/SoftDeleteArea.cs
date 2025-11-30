using UHO_API.Interfaces;
using UHO_API.Interfaces.IRepository;
using UHO_API.Utilities;

namespace UHO_API.Features.Area.Command;

public record SoftDeleteAreaCommand(int Id) : IRequest<bool>;

public class SoftDeleteAreaHandler : IRequestHandler<SoftDeleteAreaCommand, bool>
{
    private readonly IUnitOfWorks _uow;

    public SoftDeleteAreaHandler(IUnitOfWorks uow)
    {
        _uow = uow;
    }

    public async Task<Result<bool>> Handle(SoftDeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var areaToDelete = await _uow.Area.GetById(request.Id);
        
        if (areaToDelete is null)
        {
            return Error.NotFound("Área no encontrada.");
        }

        // Usamos el método de soft delete que definiste en tu repositorio
        await _uow.Area.SoftDelete(request.Id);
        await _uow.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}