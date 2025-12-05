using UHO_API.Shared.Results;

namespace UHO_API.Core.Interfaces;

public interface IRequest<TResponse> { }
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}