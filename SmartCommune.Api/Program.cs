using Serilog;

using SmartCommune.Api;
using SmartCommune.Api.Common.Constants;
using SmartCommune.Api.Common.Extensions;
using SmartCommune.Api.Middlewares;
using SmartCommune.Application;
using SmartCommune.Infrastructure;

// 1. CREATE BUILDER.
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration, builder.Environment)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// 2. BUILD APP.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors(CorsPolicyNames.App);

app.UseMiddleware<ErrorHandlingMiddleware>();

await app.MigrateDatabaseAsync();

await app.SeedDatabaseAsync();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();