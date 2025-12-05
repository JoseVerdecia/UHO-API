
using UHO_API.Core.Interfaces;
using UHO_API.Shared.Results;

namespace UHO_API.Core.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }