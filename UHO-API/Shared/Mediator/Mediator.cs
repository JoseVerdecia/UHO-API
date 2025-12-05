using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;

namespace UHO_API.Shared.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    
    public Mediator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
       
        using var scope = _serviceScopeFactory.CreateScope();
        
        
        var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        
        return handler.Handle(request, cancellationToken);
    }
}
