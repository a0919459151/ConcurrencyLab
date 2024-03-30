var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register ioc container
builder.Services
    .AddExceptionHandler<CustomExceptionHandler>()
    .AddProblemDetails()
    .AddDBContext()
    .AddRedLock()
    .AddServices();

// ignore swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Register middleware
app.UseExceptionHandler();
app.UseDataSeeding();

app.UseAuthorization();
app.MapControllers();

// ignore swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

Console.WriteLine("Server on!");

app.Run();
