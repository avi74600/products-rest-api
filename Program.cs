using Microsoft.OpenApi;
using ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers and DI services
builder.Services.AddControllers();
builder.Services.AddSingleton<IProductService, InMemoryProductService>();

// Configure OpenAPI & Swagger documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI across all environments for easy reviewer testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products REST API v1");
    // Serve Swagger UI at both root ("/") and "/swagger"
    c.RoutePrefix = string.Empty;
});

// Avoid redirect issues during CLI curl testing
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
