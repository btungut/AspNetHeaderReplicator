// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.AspNetCore.Builder;

// namespace DotNetHeaderReplicator.Tests;

// public static class IntegrationTestHelper
// {
//     public static HttpClient GetHttpClient(Action<HeaderReplicatorConfigurationBuilder> configure)
//     {
//         var builder = new WebHostBuilder()
//             .ConfigureServices(services =>
//             {
//                 services.AddHeaderReplicator(configure);
//             })
//             .Configure(app =>
//             {
//                 app.UseHeaderReplicator();
//                 app.Run(async context =>
//                 {
//                     await context.Response.WriteAsync("Hello World!");
//                 });
//             });

//         var server = new TestServer(builder);
//         return server.CreateClient();
//     }

//     public static Task<HttpResponseMessage> GetResponseAsync(HttpClient client, Action<HttpRequestMessage> configureRequest)
//     {
//         var request = new HttpRequestMessage(HttpMethod.Get, "/");
//         configureRequest?.Invoke(request);
//         return client.SendAsync(request);
//     }
// }