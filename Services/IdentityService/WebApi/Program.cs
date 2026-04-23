using Application.Interfaces;
using Application.Services;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception());

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
