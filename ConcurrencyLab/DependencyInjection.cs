using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using RedLockNet;
using StackExchange.Redis;
using Medallion.Threading.SqlServer;
using Medallion.Threading;

namespace ConcurrencyLab;

public static class DependencyInjection
{
    // AddServices
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();  // To follow Dependency Inverson Principle, registering services by interface is better.

        return services;
    }

    // AddDBContext
    public static IServiceCollection AddDBContext(this IServiceCollection services)
    {
        // connection string
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var connStr = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("DefaultConnection not found");

        services.AddDbContext<ConcurrencyLabDbContext>(options =>
        {
            // Use in memory database
            //options.UseInMemoryDatabase("ConcurrencyLab");

            // Use sql server
            options.UseSqlServer(connStr);
        });

        return services;
    }

    // AddRedLock
    public static IServiceCollection AddRedLock(this IServiceCollection services)
    {
        var multiplexer = new List<RedLockMultiplexer> {
            ConnectionMultiplexer.Connect("localhost")
        };

        var redLockFactory = RedLockFactory.Create(multiplexer);

        services.AddSingleton<IDistributedLockFactory>(redLockFactory);

        return services;
    }

    // AddSqlLcok
    public static IServiceCollection AddSqlLock(this IServiceCollection services)
    {
        // connection string
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var connStr = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("DefaultConnection not found");

        services.AddSingleton<IDistributedLockProvider>(new SqlDistributedSynchronizationProvider(connStr));

        return services;
    }
}
