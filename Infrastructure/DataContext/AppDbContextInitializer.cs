using Microsoft.EntityFrameworkCore;

namespace VocabBuilder.Infrastructure.DataContext;

public class AppDbContextInitializer(ILogger<AppDbContextInitializer> logger, AppDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
}
