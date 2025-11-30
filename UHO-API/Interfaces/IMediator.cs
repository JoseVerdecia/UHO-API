using UHO_API.Utilities;

namespace UHO_API.Interfaces;

public interface IMediator
{
    Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}