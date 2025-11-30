using Microsoft.EntityFrameworkCore;
using UHO_API.Data;

namespace UHO_API.Extensions.ConfigurationExtensions;

public static class DbExtensions
{
    public static void AddDbConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("AuthDb"));
    }
}