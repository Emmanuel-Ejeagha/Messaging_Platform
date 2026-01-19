using FluentAssertions;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;
using Xunit;

namespace MessagingPlatform.Application.Tests.UnitTests.ValueObjects;

public class MessageContentTests
{
    [Fact]
    public void Create_WithValidContent_ShouldCreateMessageContent()
    {
        // Act
        var content = MessageContent.Create("Hello, World!");

        // Assert
        content.Should().NotBeNull();
        content.Content.Should().Be("Hello, World!");
        content.HasMedia.Should().BeFalse();
        content.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public void Create_WithMedia_ShouldCreateMessageContentWithMedia()
    {
        // Act
        var content = MessageContent.CreateWithMedia("https://example.com/image.jpg", "Check this!");

        // Assert
        content.Should().NotBeNull();
        content.Content.Should().Be("Check this!");
        content.MediaUrl.Should().Be("https://example.com/image.jpg");
        content.HasMedia.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyContentAndNoMedia_ShouldThrowException(string emptyContent)
    {
        // Act & Assert
        var action = () => MessageContent.Create(emptyContent);
        action.Should().Throw<DomainException>()
            .WithMessage("*Message must have either content or media*");
    }

    [Fact]
    public void Create_WithContentTooLong_ShouldThrowException()
    {
        // Arrange
        var longContent = new string('a', 5001);

        // Act & Assert
        var action = () => MessageContent.Create(longContent);
        action.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 5000 characters*");
    }

    [Fact]
    public void Create_WithMediaUrlTooLong_ShouldThrowException()
    {
        // Arrange
        var longUrl = new string('a', 2049);

        // Act & Assert
        var action = () => MessageContent.CreateWithMedia(longUrl);
        action.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 2048 characters*");
    }

    [Fact]
    public void TwoMessageContents_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var content1 = MessageContent.Create("Hello");
        var content2 = MessageContent.Create("Hello");

        // Assert
        content1.Should().Be(content2);
        content1.GetHashCode().Should().Be(content2.GetHashCode());
    }

    [Fact]
    public void TwoMessageContents_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var content1 = MessageContent.Create("Hello");
        var content2 = MessageContent.Create("World");

        // Assert
        content1.Should().NotBe(content2);
    }
}