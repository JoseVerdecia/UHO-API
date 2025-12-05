using UHO_API.Interfaces;
using UHO_API.Utilities;

namespace UHO_API.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }