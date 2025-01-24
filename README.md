# AspNetHeaderReplicator

This is a simple library that exposes a middleware for ASP.NET Core applications that **replicates headers from the request to the response** with the ability to **include or exclude** specific headers.

## Installation

AspNetHeaderReplicator is available as a NuGet package. You can install it using the NuGet Package Manager Console:

```bash
dotnet add package AspNetHeaderReplicator
```

Or with the `nuget` command:

```bash
nuget install AspNetHeaderReplicator
```

---

## Usage

To use the middleware, you need to add it to the pipeline in the `Configure` method of your `Startup` class (Or you can use without the `Startup` class if you are using the generic host):

### Default configuration

By default, the middleware will replicate and ignore the following headers:

#### Replicated headers

```csharp
new[] { "X-", "My-", "Req-", "Trace-", "Debug-", "Verbose-" }
```

#### Ignored headers

```csharp
new[] { "auth", "credential", "token", "pass", "secret", "hash", "cert" };
```

---

### Example 01: Use default configuration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHeaderReplicator();
}
```

---

### Example 02: Use default configuration AND add custom allowed headers

Following code snippet will allow headers starting with `Allowed-Prefix` to be replicated.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHeaderReplicator(opt =>
    {
        // Allow headers starting with
        opt.AllowHeaderPrefix("Allowed-Prefix");
    });
}
```

---

### Example 03: Use default configuration AND add custom ignored headers

Following code snippet will ignore headers containing `ignored` in their name.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHeaderReplicator(opt =>
    {
        // Ignore headers containing
        opt.IgnoreHeaderSentence("ignored");
    });
}
```

---

### Example 04: Clear default configuration AND add custom allowed headers with custom ignored headers

Following code snippet will clear all default settings and allow headers starting with `X-` and `My-` and ignore headers containing `auth` and `credential`.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHeaderReplicator(opt =>
    {
        // Clear all default settings
        opt.ClearAll();

        // Allow headers starting with
        opt.AllowHeaderPrefix("X-");
        opt.AllowHeaderPrefix("My-");

        // Ignore headers containing
        opt.IgnoreHeaderSentence("auth");
        opt.IgnoreHeaderSentence("credential");
    });
}
```

---
<br /><br />

## Demo

You can create a new API project and apply the following changes...

### Demo: Startup.cs

```csharp
...
...
public void ConfigureServices(IServiceCollection services)
{
    services.AddHeaderReplicator(opt =>
    {
        // Clear all default settings
        opt.ClearAll();

        // Allow headers starting with
        opt.AllowHeaderPrefix("X-");
        opt.AllowHeaderPrefix("My-");

        // Ignore headers containing
        opt.IgnoreHeaderSentence("auth");
        opt.IgnoreHeaderSentence("credential");
    });
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHeaderReplicator();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.Map("/", context =>
        {
            return context.Response.WriteAsync("Please inspect the headers of this response.");
        });
    });
}
...
...
```

### Demo: Test the API

Run the API and make a request to the root URL. You can use `curl` or any other tool to inspect the headers of the response.

```bash
curl --location 'http://localhost:5278/' \
    --header 'X-Burak: Burak Tungut' \
    --header 'My-Some: 123' \
    --header 'Some-Auth-Key: somesecrets' \
    --header 'a-header-credential-demo: somesecrets'
```

![request headers](https://raw.githubusercontent.com/btungut/AspNetHeaderReplicator/refs/heads/master/.img/request_headers.png)

> As you can see, headers are being executing without case sensitivity.

Response headers will be like below:

![response headers](https://raw.githubusercontent.com/btungut/AspNetHeaderReplicator/refs/heads/master/.img/response_headers.png)

> As you can see, headers are being replicated and/or ignored according to the configuration.

---
<br /><br />

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

# Contributing

Owner : **Burak Tungut**

Any contributions are welcome! Please post your issues and pull requests to the repository.

Regards...