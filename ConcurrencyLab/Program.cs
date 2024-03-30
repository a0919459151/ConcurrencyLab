using ConcurrencyLab.Middleware.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ExceptionHandler
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

// EF Core inmemery
builder.Services.AddDbContext<ConcurrencyLabDbContext>(options =>
{
    options.UseInMemoryDatabase("ConcurrencyLab");
});

// Services
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Middleware
app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Data seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ConcurrencyLabDbContext>();
        context.Database.EnsureCreated();
        SeedData(context);
        logger.LogInformation("Data seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();


#region Data seeding
static void SeedData(ConcurrencyLabDbContext context)
{
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Id = 1, Name = "§j¶Pπq¡Á", Amount = 100 }
        );
        context.SaveChanges();
    }
}
#endregion