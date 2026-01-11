using SmartCommune.Api;
using SmartCommune.Application;
using SmartCommune.Infrastructure;

// 1. CREATE BUILDER
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Environment)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// 2. BUILD APP
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();