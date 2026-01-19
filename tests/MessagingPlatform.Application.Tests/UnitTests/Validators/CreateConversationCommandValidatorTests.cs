using FluentValidation.TestHelper;
using MessagingPlatform.Application.Conversations.Commands.CreateConversation;
using MessagingPlatform.Domain.Enums;
using Xunit;

namespace MessagingPlatform.Application.Tests.UnitTests.Validators;

public class CreateConversationCommandValidatorTests
{
    private readonly CreateConversationCommandValidator _validator;

    public CreateConversationCommandValidatorTests()
    {
        _validator = new CreateConversationCommandValidator();
    }

    [Fact]
    public void Should_HaveError_WhenCreatorIdIsEmpty()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.Empty,
            Type = ConversationType.OneOnOne,
            ParticipantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatorId);
    }

    [Fact]
    public void Should_HaveError_WhenOneOnOneConversationHasWrongParticipantCount()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.NewGuid(),
            Type = ConversationType.OneOnOne,
            ParticipantIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParticipantIds)
            .WithErrorMessage("One-on-one conversations must have exactly 2 participants");
    }

    [Fact]
    public void Should_HaveError_WhenGroupConversationHasNoTitle()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.NewGuid(),
            Type = ConversationType.Group,
            Title = "",
            ParticipantIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required for group conversations");
    }

    [Fact]
    public void Should_HaveError_WhenGroupConversationTitleTooLong()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.NewGuid(),
            Type = ConversationType.Group,
            Title = new string('a', 101),
            ParticipantIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 100 characters");
    }

    [Fact]
    public void Should_NotHaveErrors_WhenValidOneOnOneConversation()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.NewGuid(),
            Type = ConversationType.OneOnOne,
            Title = "Chat",
            ParticipantIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_WhenValidGroupConversation()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            CreatorId = Guid.NewGuid(),
            Type = ConversationType.Group,
            Title = "Group Chat",
            ParticipantIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}