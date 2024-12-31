/// <summary>
/// Represents the unit tests for the <see cref="HeaderReplicatorMiddleware"/> class only for ctor and InvokeAsync methods.
/// </summary>
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetHeaderReplicator.Tests;

public class HeaderReplicatorMiddlewareUnitTests
{
    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigIsNull()
    {
        // Arrange
        IHeaderReplicatorConfiguration config = null;
        LoggerFactory loggerFactory = new Mock<LoggerFactory>().Object;

        // Act
        Action act = () => new HeaderReplicatorMiddleware(config, loggerFactory);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenLoggerFactoryIsNull()
    {
        // Arrange
        IHeaderReplicatorConfiguration config = new HeaderReplicatorConfiguration(true, new HashSet<string>(), new HashSet<string>());
        LoggerFactory loggerFactory = null;

        // Act
        Action act = () => new HeaderReplicatorMiddleware(config, loggerFactory);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public async Task InvokeAsync_ShouldThrowException_WhenContextIsNull()
    {
        // Arrange
        IHeaderReplicatorConfiguration config = new HeaderReplicatorConfiguration(true, new HashSet<string>(), new HashSet<string>());
        LoggerFactory loggerFactory = new Mock<LoggerFactory>().Object;
        HeaderReplicatorMiddleware middleware = new HeaderReplicatorMiddleware(config, loggerFactory);
        HttpContext context = null;
        RequestDelegate next = (_) => Task.CompletedTask;

        // Act
        Func<Task> act = async () => await middleware.InvokeAsync(context, next);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task InvokeAsync_ShouldThrowException_WhenNextIsNull()
    {
        // Arrange
        IHeaderReplicatorConfiguration config = new HeaderReplicatorConfiguration(true, new HashSet<string>(), new HashSet<string>());
        LoggerFactory loggerFactory = new Mock<LoggerFactory>().Object;
        HeaderReplicatorMiddleware middleware = new HeaderReplicatorMiddleware(config, loggerFactory);
        HttpContext context = new DefaultHttpContext();
        RequestDelegate next = null;

        // Act
        Func<Task> act = async () => await middleware.InvokeAsync(context, next);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
}