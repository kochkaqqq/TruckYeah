using ListingService.Application.Interfaces;
using ListingService.Application.Services;
using ListingService.Infrastructure;
using ListingService.Infrastructure.Implementation;
using ListingService.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ListingServiceDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ListingServiceDbContext)));
});

builder.Services.AddScoped<ICargosRepository, CargosRepository>();
builder.Services.AddScoped<ITrucksRepository, TrucksRepository>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<ITrucksService, TrucksService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();

