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

// Middleware này chỉ làm nhiệm vụ: "Cố gắng định danh người dùng".
// Nếu request có Token hợp lệ -> Nó set User.Identity.IsAuthenticated = true.
// Nếu request không có Token (ví dụ: khách vãng lai, hoặc đang gọi API Login/Register) -> Nó cho qua và set User là "Anonymous" (Vô danh, IsAuthenticated = false).
// Nó không chặn request lại. Việc chặn (trả về 401/403) là nhiệm vụ của app.UseAuthorization() hoặc các Attribute [Authorize] ở Controller.
app.UseAuthentication();

app.UseMiddleware<SecurityStampMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();