namespace ConcurrencyLab.Middleware.DataSeedMiddleware;

public static class DataSeedingMiddlewareExtension
{
    public static IApplicationBuilder UseDataSeeding(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DataSeedingMiddleware>();
    }
}