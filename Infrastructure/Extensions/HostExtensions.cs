using VocabBuilder.Infrastructure.DataContext;

namespace VocabBuilder.Infrastructure.Extensions;

public static class HostExtensions
{
    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
        await initializer.InitialiseAsync().ConfigureAwait(false);
    }
}
