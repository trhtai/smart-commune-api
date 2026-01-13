using Microsoft.EntityFrameworkCore;

using SmartCommune.Infrastructure.Persistence;

namespace SmartCommune.Api.Common.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Found {Count} pending migrations:", pendingMigrations.Count());

                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation(" - Applying migration: {Migration}", migration);
                }

                await dbContext.Database.MigrateAsync();
                logger.LogInformation("All pending migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found. Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while applying migrations or seeding");
            throw;
        }
    }

    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>();
        await seeder.SeedAsync(app.Lifetime.ApplicationStopping);
    }
}