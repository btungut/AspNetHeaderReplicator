/// <summary>
/// Represents the unit tests for the <see cref="HeaderReplicatorConfiguration"/> class.
/// </summary>
namespace DotNetHeaderReplicator.Tests;

public class HeaderReplicatorConfigurationUnitTests
{
    [Fact]
    public void Ctor_ShouldThrowException_WhenAllowAllIsFalse_And_AllowedPrefixesIsNull()
    {
        // Arrange
        bool isAllowAll = false;
        HashSet<string> allowedPrefixes = null;
        HashSet<string> ignoredSentences = new HashSet<string>(Helpers.GetEmptyEnumerable());

        // Act
        Action act = () => new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenAllowAllIsFalse_And_IgnoredSentencesIsNull()
    {
        // Arrange
        bool isAllowAll = false;
        HashSet<string> allowedPrefixes = new HashSet<string>(Helpers.GetEmptyEnumerable()); ;
        HashSet<string> ignoredSentences = null;

        // Act
        Action act = () => new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenAllowAllIsTrue_And_AllowedPrefixesIsEmpty()
    {
        // Arrange
        bool isAllowAll = true;
        HashSet<string> allowedPrefixes = new HashSet<string>(Helpers.GetEmptyEnumerable());
        HashSet<string> ignoredSentences = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };

        // Act
        Action act = () => new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenAllowAllIsTrue_And_IgnoredSentencesIsEmpty()
    {
        // Arrange
        bool isAllowAll = true;
        HashSet<string> allowedPrefixes = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };
        HashSet<string> ignoredSentences = new HashSet<string>(Helpers.GetEmptyEnumerable());

        // Act
        Action act = () => new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenAllowAllIsTrue_And_AllowedPrefixesAndIgnoredSentencesAreNotEmpty()
    {
        // Arrange
        bool isAllowAll = true;
        HashSet<string> allowedPrefixes = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };
        HashSet<string> ignoredSentences = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };

        // Act
        Action act = () => new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Ctor_ShouldCreateInstance_WhenAllowAllIsFalse_And_AllowedPrefixesAndIgnoredSentencesAreEmpty()
    {
        // Arrange
        bool isAllowAll = false;
        HashSet<string> allowedPrefixes = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };
        HashSet<string> ignoredSentences = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };

        // Act
        var config = new HeaderReplicatorConfiguration(isAllowAll, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.NotNull(config);
        Assert.Equal(isAllowAll, config.AllowAll);
        Assert.Equal(allowedPrefixes.OrderBy(x => x), config.AllowedHeaderPrefixes.OrderBy(x => x));
        Assert.Equal(ignoredSentences.OrderBy(x => x), config.IgnoredHeaderSentences.OrderBy(x => x));
    }

}