using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UHO_API.Infraestructure.Data;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class DbExtensions
{
    public static void AddDbConfiguration(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("AuthDb");
            options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }
}