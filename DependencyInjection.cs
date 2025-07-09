using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VocabBuilder.Infrastructure.DataContext;
using VocabBuilder.Infrastructure.Interceptors;
using VocabBuilder.Infrastructure.Providers.CambridgeDictionary;
using VocabBuilder.Infrastructure.Providers.GoogleTts;
using VocabBuilder.Infrastructure.Repositories.Phonetic;
using VocabBuilder.Infrastructure.Repositories.Vocab;
using VocabBuilder.Services.Export;
using VocabBuilder.Services.Navigation;
using VocabBuilder.Services.Vocab;
using VocabBuilder.Shared.Configurations;

namespace VocabBuilder;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration sections
        services.Configure<AppOptions>(configuration.GetSection("App"));
        services.Configure<PixabayOptions>(configuration.GetSection("Pixabay"));

        // Register database context
        services.AddDbContextFactory<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(new AuditingInterceptor());
        });
        services.AddScoped<AppDbContextInitializer>();

        services.AddHttpClient();

        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IVocabService, VocabService>();
        services.AddScoped<ICsvExporter, CsvExporter>();

        services.AddScoped<IVocabRepository, VocabRepository>();
        services.AddScoped<IPhoneticRepository, PhoneticRepository>();
        services.AddScoped<ICambridgeDictionaryProvider, CambridgeDictionaryProvider>();
        services.AddScoped<IGoogleTtsProvider, GoogleTtsProvider>();

        return services;
    }
}
