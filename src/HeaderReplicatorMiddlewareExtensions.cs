/// <summary>
/// Represents the extension methods for the <see cref="IServiceCollection"/> interface which are used to add <see cref="HeaderReplicatorMiddleware"/> in fluent way.
/// </summary>
/// 
using DotNetHeaderReplicator;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class HeaderReplicatorMiddlewareExtensions
{
    public static IServiceCollection AddHeaderReplicator(this IServiceCollection services)
    {
        return AddHeaderReplicator(services, _ => { });
    }

    public static IServiceCollection AddHeaderReplicator(this IServiceCollection services, Action<HeaderReplicatorConfigurationBuilder> configure)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        var builder = new HeaderReplicatorConfigurationBuilder();
        configure.Invoke(builder);
        var opt = builder.Build();

        services.AddSingleton<IHeaderReplicatorConfiguration>(opt);
        services.AddScoped<HeaderReplicatorMiddleware>();

        return services;
    }

    public static IApplicationBuilder UseHeaderReplicator(this IApplicationBuilder app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        return app.UseMiddleware<HeaderReplicatorMiddleware>();
    }
}