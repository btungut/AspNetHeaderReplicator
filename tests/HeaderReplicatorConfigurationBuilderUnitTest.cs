namespace DotNetHeaderReplicator.Tests;

public class HeaderReplicatorConfigurationBuilderUnitTest
{
    HeaderReplicatorConfigurationBuilder builder = new HeaderReplicatorConfigurationBuilder();

    [Fact]
    public void ClearAll_ShouldThrowException_WhenAllowAllIsCalled()
    {
        // Act
        Action act = () => builder.AllowAll().ClearAll();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AllowHeaderPrefix_ShouldReturnsAddedPrefix_WhenBuilderIsCalled()
    {
        // Arrange
        var prefix = Helpers.GetRandomValue();

        // Act
        var result = builder.AllowHeaderPrefix(prefix);
        var built = builder.Build();

        // Assert
        Assert.IsType<HeaderReplicatorConfigurationBuilder>(result);
        Assert.Contains(prefix, built.AllowedHeaderPrefixes);
    }

    [Fact]
    public void IgnoreHeaderSentence_ShouldReturnsAddedSentence_WhenBuilderIsCalled()
    {
        // Arrange
        var sentence = Helpers.GetRandomValue();

        // Act
        var result = builder.IgnoreHeaderSentence(sentence);
        var built = builder.Build();

        // Assert
        Assert.IsType<HeaderReplicatorConfigurationBuilder>(result);
        Assert.Contains(sentence, built.IgnoredHeaderSentences);
    }

    [Fact]
    public void ClearAllowedHeaderPrefixes_ShouldReturnsEmptyAllowedHeaderPrefixes_WhenBuilderIsCalled()
    {
        // Arrange
        builder.AllowHeaderPrefix(Helpers.GetRandomValue());

        // Act
        var result = builder.ClearAllowedHeaderPrefixes();
        var built = builder.Build();

        // Assert
        Assert.IsType<HeaderReplicatorConfigurationBuilder>(result);
        Assert.Empty(built.AllowedHeaderPrefixes);
    }

    [Fact]
    public void ClearIgnoredHeaderSentences_ShouldReturnsEmptyIgnoredHeaderSentences_WhenBuilderIsCalled()
    {
        // Arrange
        builder.IgnoreHeaderSentence(Helpers.GetRandomValue());

        // Act
        var result = builder.ClearIgnoredHeaderSentences();
        var built = builder.Build();

        // Assert
        Assert.IsType<HeaderReplicatorConfigurationBuilder>(result);
        Assert.Empty(built.IgnoredHeaderSentences);
    }

    [Fact]
    public void ClearAll_ShouldReturnsEmptyAllowedHeaderPrefixesAndIgnoredHeaderSentences_WhenBuilderIsCalled()
    {
        // Arrange
        builder.AllowHeaderPrefix(Helpers.GetRandomValue());
        builder.IgnoreHeaderSentence(Helpers.GetRandomValue());

        // Act
        var result = builder.ClearAll();
        var built = builder.Build();

        // Assert
        Assert.IsType<HeaderReplicatorConfigurationBuilder>(result);
        Assert.Empty(built.AllowedHeaderPrefixes);
        Assert.Empty(built.IgnoredHeaderSentences);
    }

    [Fact]
    public void Build_ShouldReturnsBuiltConfiguration_WhenBuilderIsCalledWithClearAllAndCustomValues()
    {
        // Arrange
        var allowedPrefixes = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };
        var ignoredSentences = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };

        // Act
        var result = builder
            .ClearAll()
            .AllowHeaderPrefixes(allowedPrefixes)
            .IgnoreHeaderSentences(ignoredSentences)
            .Build();

        // Assert
        Assert.True(Helpers.AreStringCollectionContainsWithoutOrderAndCase(allowedPrefixes, result.AllowedHeaderPrefixes));
        Assert.True(Helpers.AreStringCollectionsEqualWithoutOrderAndCase(allowedPrefixes, result.AllowedHeaderPrefixes));
    }

    [Fact]
    public void Build_ShouldReturnsBuiltConfiguration_WhenBuilderIsCalledDefaultValuesWithCustomValues()
    {
        // Arrange
        var allowedPrefixes = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };
        var ignoredSentences = new HashSet<string>
        {
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue(),
            Helpers.GetRandomValue()
        };

        // Act
        var result = builder
            .AllowHeaderPrefixes(allowedPrefixes)
            .IgnoreHeaderSentences(ignoredSentences)
            .Build();

        // Assert
        Assert.True(Helpers.AreStringCollectionContainsWithoutOrderAndCase(allowedPrefixes, result.AllowedHeaderPrefixes));
    }
}