using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;

namespace UHO_API.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }