using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RouteService.Application.Interfaces;
using RouteService.Application.Services;
using RouteService.Infrastructure;
using RouteService.Infrastructure.Implementation;
using RouteService.Infrastructure.Interfaces;
using RouteService.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RouteService API", Version = "v1" });
});

builder.Services.Configure<OpenRouteServiceOptions>(builder.Configuration.GetSection("OpenRouteService"));
builder.Services.Configure<RouteCalculationOptions>(builder.Configuration.GetSection("RouteCalculation"));

builder.Services.AddDbContext<RouteServiceDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(RouteServiceDbContext)));
});

builder.Services.AddHttpClient<IRouteProvider, OpenRouteServiceRouteProvider>((serviceProvider, client) =>
{
    var routeOptions = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<RouteCalculationOptions>>()
        .Value;

    client.Timeout = TimeSpan.FromSeconds(Math.Max(routeOptions.RequestTimeoutSeconds, 1));
});

builder.Services.AddScoped<IRouteCalculationsRepository, RouteCalculationsRepository>();
builder.Services.AddScoped<IRouteCalculationService, RouteCalculationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RouteServiceDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
