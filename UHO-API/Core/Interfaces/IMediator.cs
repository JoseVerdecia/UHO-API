using UHO_API.Shared.Results;

namespace UHO_API.Core.Interfaces;

public interface IMediator
{
    Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}