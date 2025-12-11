using FluentValidation;
using UHO_API.Features.Area.Validations;
using UHO_API.Features.Authentication.Validations;
using UHO_API.Features.Users.Validations;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class ValidationExtensions
{
    public static void AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<AreaModelValidator>();
        services.AddValidatorsFromAssemblyContaining<DegradeUserValidator>();
    }
}