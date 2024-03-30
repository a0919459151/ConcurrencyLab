﻿using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using RedLockNet;
using StackExchange.Redis;

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
        services.AddDbContext<ConcurrencyLabDbContext>(options =>
        {
            options.UseInMemoryDatabase("ConcurrencyLab");
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
}
