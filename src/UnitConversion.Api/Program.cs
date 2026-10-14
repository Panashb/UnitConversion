using UnitConversion.Api.Middleware;
using UnitConversion.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();
builder.Services.AddUnitConversion();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // /swagger/v1/swagger.json
    app.UseSwaggerUI();     // /swagger
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Exposed so integration tests can use WebApplicationFactory<Program>.
public partial class Program;
