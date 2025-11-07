using Microsoft.EntityFrameworkCore;
using ExamenDiegoUnidad2.Data;
using ExamenDiegoUnidad2.Services.Interfaces;
using ExamenDiegoUnidad2.Services.Implementations;
using ExamenDiegoUnidad2.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework
builder.Services.AddDbContext<ExamenDiegoUnidad2.Data.AppContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql")
    ));

// Register services
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<ICarreraService, CarreraService>();
builder.Services.AddScoped<ITipoEstudianteService, TipoEstudianteService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "UnivSys API",
        Version = "v1",
        Description = "API para el sistema de registro de estudiantes UnivSys"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UnivSys API V1");
    });
}

// Add custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
