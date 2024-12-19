using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton<HeaderReplicatorConfigurationBuilder.HeaderReplicatorConfiguration>(opt);
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