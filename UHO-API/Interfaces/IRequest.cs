using UHO_API.Utilities;

namespace UHO_API.Interfaces;

public interface IRequest<TResponse> { }
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}