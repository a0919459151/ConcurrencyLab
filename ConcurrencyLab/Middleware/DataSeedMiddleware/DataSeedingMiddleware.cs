namespace ConcurrencyLab.Middleware.DataSeedMiddleware;

public class DataSeedingMiddleware
{
    private readonly RequestDelegate _next;

    public DataSeedingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Invoke
    public async Task InvokeAsync(HttpContext context, ConcurrencyLabDbContext dbContext, IConfiguration configuration)
    {
        var productAmount = configuration.GetSection("ProductAmount").Get<int?>() ?? 10;

        await SeedProduct(dbContext, productAmount);

        await _next(context);
    }

    // Seed product
    public async Task SeedProduct(ConcurrencyLabDbContext context, int productAmount)
    {
        context.Database.EnsureCreated();

        if (!context.Products.Any())
        {
            var newProduct = new Product { Id = 1, Name = "大同電鍋", OriginalAmount = productAmount, Amount = productAmount };

            await context.Products.AddAsync(newProduct);

            await context.SaveChangesAsync();

            Console.WriteLine($"Data seed: {{ Id = 1,Name = 大同電鍋, OriginalAmount = {productAmount}, Amount = {productAmount} }}");

            Console.WriteLine("Data seeding completed.");
        }
    }
}
