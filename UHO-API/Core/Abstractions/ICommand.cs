using UHO_API.Interfaces;
using UHO_API.Utilities;

namespace UHO_API.Core.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }