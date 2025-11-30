using UHO_API.Interfaces;

namespace UHO_API.Utilities;

public class Mediator : IMediator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // ✅ CAMBIO 1: Inyecta IServiceScopeFactory en lugar de IServiceProvider
    public Mediator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        // ✅ CAMBIO 2: Crea un ámbito temporal para resolver el handler
        using var scope = _serviceScopeFactory.CreateScope();
        
        // Resuelve el handler DESDE el ámbito seguro
        var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        
        return handler.Handle(request, cancellationToken);
    }
}