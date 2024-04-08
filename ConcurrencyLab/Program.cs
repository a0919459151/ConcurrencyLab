
var builder = WebApplication.CreateBuilder(args);

// Register ioc container
builder.Services
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddProblemDetails()
    .AddDBContext()
    .AddRedLock()
    .AddSqlLock()
    .AddServices()
    .AddControllers();

var app = builder.Build();

// Register middleware
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

// Data seeding init, while server start
//await SeedProductForInMemory();
await SeedProductForSqlServer();

Console.WriteLine("Server on!");

app.Run();

#region Data seeding
// Seed product for in memory efcore provider
async Task SeedProductForInMemory()
{
    using var scope = app.Services.CreateScope();

    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var productAmount = configuration.GetSection("ProductAmount").Get<int?>() ?? 10;

    var dbContext = scope.ServiceProvider.GetRequiredService<ConcurrencyLabDbContext>();
    dbContext.Database.EnsureCreated();

    var newProduct = new Product { Id = 1, Name = "大同電鍋", OriginalAmount = productAmount, Amount = productAmount };
    await dbContext.Products.AddAsync(newProduct);
    await dbContext.SaveChangesAsync();

    Console.WriteLine($"Data seed: {{ Id = {newProduct.Id},Name = 大同電鍋, OriginalAmount = {productAmount}, Amount = {productAmount} }}");
    Console.WriteLine("Data seeding completed.");
}

// Seed product for sql server efcore provider
async Task SeedProductForSqlServer()
{
    using var scope = app.Services.CreateScope();

    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var productAmount = configuration.GetSection("ProductAmount").Get<int?>() ?? 10;

    var dbContext = scope.ServiceProvider.GetRequiredService<ConcurrencyLabDbContext>();
    dbContext.Database.EnsureCreated();

    var product = dbContext.Products.Find(1);

    // Create
    if (product is null)
    {
        var newProduct = new Product { Name = "大同電鍋", OriginalAmount = productAmount, Amount = productAmount };

        await dbContext.Products.AddAsync(newProduct);

        await dbContext.SaveChangesAsync();
    }
    // Update
    else
    {
        product.Amount = productAmount;

        await dbContext.SaveChangesAsync();
    }

    Console.WriteLine($"Data seed: {{ Id = {product!.Id},Name = 大同電鍋, OriginalAmount = {productAmount}, Amount = {productAmount} }}");
    Console.WriteLine("Data seeding completed.");
}
#endregion