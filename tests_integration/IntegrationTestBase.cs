using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;

namespace AspNetHeaderReplicator.IntegrationTests;

public class IntegrationTestBuilder : IDisposable
{
    private Action<HeaderReplicatorConfigurationBuilder> _configurationBuilderDelegate;
    private IHost _host;
    private bool _isDisposed;

    private IntegrationTestBuilder(Action<HeaderReplicatorConfigurationBuilder> configurationBuilderDelegate)
    {
        _configurationBuilderDelegate = configurationBuilderDelegate ?? throw new ArgumentNullException(nameof(configurationBuilderDelegate));
    }

    public static async Task<IntegrationTestBuilder> CreateAsync(Action<HeaderReplicatorConfigurationBuilder> configurationBuilderDelegate)
    {
        var instance = new IntegrationTestBuilder(configurationBuilderDelegate);
        await instance.CreateNewHostAsync();
        return instance;
    }

    public HttpClient GetClient()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_host == null) throw new InvalidOperationException($"Host not created yet! Please call {nameof(CreateAsync)} before calling {nameof(GetClient)}");
        return _host.GetTestClient();
    }

    private async Task CreateNewHostAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        if (_host != null) throw new InvalidOperationException("Host already created!");

        var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddHeaderReplicator(_configurationBuilderDelegate ?? throw new InvalidOperationException($"Configuration builder delegate not set!"));
                    })
                    .Configure(app =>
                    {
                        app.UseHeaderReplicator();
                        app.Run(async httpContext =>
                        {
                            var headerReplicatorConfig = httpContext.RequestServices.GetRequiredService<IHeaderReplicatorConfiguration>() ?? throw new ArgumentNullException(nameof(IHeaderReplicatorConfiguration));

                            var returnObj = new
                            {
                                RespondedBy = $"{nameof(IntegrationTestBuilder)}.{nameof(CreateNewHostAsync)}",
                                RequestHeaders = httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                                HeaderReplicatorConfig = headerReplicatorConfig,
                                StatusCode = (int)httpContext.Response.StatusCode,
                                Path = httpContext.Request.Path.Value,
                            };

                            await httpContext.Response.WriteAsJsonAsync(returnObj);
                        });
                    });
            })
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddJsonConsole();
            })
            .StartAsync();

        _host = host;
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _host?.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}