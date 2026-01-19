

using FluentAssertions;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Tests.UnitTests.Conversations;

public class ConversationTests
{
    [Fact]
    public void CreateOneOnOne_WithValidUsers_ShouldCreateConversation()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());
        var user2 = UserId.Create(Guid.NewGuid());

        // Act
        var conversation = Conversation.CreateOneOnOne(user1, user2);

        // Assert
        conversation.Should().NotBeNull();
        conversation.Type.Should().Be(ConversationType.OneOnOne);
        conversation.Participants.Should().HaveCount(2);
        conversation.Title.Should().Contain(user1.Value.ToString());
        conversation.Title.Should().Contain(user2.Value.ToString());
        conversation.IsArchived.Should().BeFalse();
    }

    [Fact]
    public void CreateOneOnOne_WithSameUser_ShouldThrowException()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());

        // Act & Assert
        var action = () => Conversation.CreateOneOnOne(user1, user1);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddMessage_ShouldUpdateLastMessageAt()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());
        var user2 = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateOneOnOne(user1, user2);
        var messageId = Guid.NewGuid();
        var initialLastMessageAt = conversation.LastMessageAt;

        // Act
        conversation.AddMessage(messageId);

        // Assert
        conversation.LastMessageAt.Should().BeAfter(initialLastMessageAt);
        conversation.MessageIds.Should().Contain(messageId);
    }

    [Fact]
    public void AddParticipant_ToGroupConversation_ShouldSucceed()
    {
        // Arrange
        var creator = UserId.Create(Guid.NewGuid());
        var newUser = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateGroup("Test Group", creator);

        // Act
        conversation.AddParticipant(newUser, ParticipantRole.Member);

        // Assert
        conversation.Participants.Should().HaveCount(2);
        conversation.GetParticipant(newUser).Should().NotBeNull();
    }

    [Fact]
    public void AddParticipant_ToOneOnOne_WhenAlreadyTwoParticipants_ShouldThrow()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());
        var user2 = UserId.Create(Guid.NewGuid());
        var user3 = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateOneOnOne(user1, user2);

        // Act & Assert
        var action = () => conversation.AddParticipant(user3);
        action.Should().Throw<Domain.Exceptions.DomainException>()
            .WithMessage("*cannot have more than 2 participants*");
    }

    [Fact]
    public void UpdateTitle_ByAdmin_ShouldUpdateTitle()
    {
        // Arrange
        var creator = UserId.Create(Guid.NewGuid());
        var admin = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateGroup("Original Title", creator);
        conversation.AddParticipant(admin, ParticipantRole.Admin);

        // Act
        conversation.UpdateTitle("New Title", admin);

        // Assert
        conversation.Title.Should().Be("New Title");
    }

    [Fact]
    public void UpdateTitle_ByRegularMember_ShouldThrowException()
    {
        // Arrange
        var creator = UserId.Create(Guid.NewGuid());
        var member = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateGroup("Original Title", creator);
        conversation.AddParticipant(member, ParticipantRole.Member);

        // Act & Assert
        var action = () => conversation.UpdateTitle("New Title", member);
        action.Should().Throw<Domain.Exceptions.DomainException>()
            .WithMessage("*Only admins and owners can update conversation title*");
    }

    [Fact]
    public void Archive_ShouldSetIsArchivedToTrue()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());
        var user2 = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateOneOnOne(user1, user2);

        // Act
        conversation.Archive();

        // Assert
        conversation.IsArchived.Should().BeTrue();
    }

    [Fact]
    public void Unarchive_ShouldSetIsArchivedToFalse()
    {
        // Arrange
        var user1 = UserId.Create(Guid.NewGuid());
        var user2 = UserId.Create(Guid.NewGuid());
        var conversation = Conversation.CreateOneOnOne(user1, user2);
        conversation.Archive();

        // Act
        conversation.Unarchive();

        // Assert
        conversation.IsArchived.Should().BeFalse();
    }
}