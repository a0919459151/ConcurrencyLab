var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register ioc container
builder.Services
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddProblemDetails()
    .AddDBContext()
    .AddRedLock()
    .AddServices();

var app = builder.Build();

// Register middleware
app.UseExceptionHandler();
app.UseDataSeeding();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Server on!");

app.Run();
