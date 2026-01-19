using FluentAssertions;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;
using Xunit;

namespace MessagingPlatform.Application.Tests.UnitTests.Messages;

public class MessageTests
{
    [Fact]
    public void Create_WithValidContent_ShouldCreateMessage()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());

        // Act
        var message = Message.Create(conversationId, senderId, "Hello, World!");

        // Assert
        message.Should().NotBeNull();
        message.ConversationId.Should().Be(conversationId);
        message.SenderId.Should().Be(senderId.Value);
        message.Content.Content.Should().Be("Hello, World!");
        message.Status.Should().Be(MessageStatus.Sent);
        message.IsEdited.Should().BeFalse();
    }

    [Fact]
    public void Create_WithMedia_ShouldCreateMessageWithMedia()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());

        // Act
        var message = Message.Create(
            conversationId, 
            senderId, 
            "Check this out!", 
            "https://example.com/image.jpg");

        // Assert
        message.Should().NotBeNull();
        message.Content.HasMedia.Should().BeTrue();
        message.Content.MediaUrl.Should().Be("https://example.com/image.jpg");
    }

    [Fact]
    public void Create_WithEmptyContentAndNoMedia_ShouldThrowException()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());

        // Act & Assert
        var action = () => Message.Create(conversationId, senderId, "");
        action.Should().Throw<Domain.Exceptions.DomainException>()
            .WithMessage("*Message must have either content or media*");
    }

    [Fact]
    public void CreateReply_ShouldSetParentMessageId()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var parentMessageId = Guid.NewGuid();

        // Act
        var reply = Message.CreateReply(
            conversationId, 
            senderId, 
            parentMessageId, 
            "This is a reply");

        // Assert
        reply.Should().NotBeNull();
        reply.ParentMessageId.Should().Be(parentMessageId);
        reply.IsReply.Should().BeTrue();
    }

    [Fact]
    public void UpdateContent_Within24Hours_ShouldUpdateContent()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Original content");

        // Act
        message.UpdateContent("Updated content", senderId);

        // Assert
        message.Content.Content.Should().Be("Updated content");
        message.IsEdited.Should().BeTrue();
    }

    [Fact]
    public void UpdateContent_ByDifferentUser_ShouldThrowException()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var otherUserId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Original content");

        // Act & Assert
        var action = () => message.UpdateContent("Updated content", otherUserId);
        action.Should().Throw<Domain.Exceptions.DomainException>()
            .WithMessage("*Only the message sender can edit the message*");
    }

    [Fact]
    public void MarkAsDelivered_ShouldUpdateStatus()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Test message");

        // Act
        message.MarkAsDelivered();

        // Assert
        message.Status.Should().Be(MessageStatus.Delivered);
        message.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsRead_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Test message");

        // Act
        message.MarkAsRead();

        // Assert
        message.Status.Should().Be(MessageStatus.Read);
        message.ReadAt.Should().NotBeNull();
        message.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatus()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Test message");

        // Act
        message.MarkAsFailed();

        // Assert
        message.Status.Should().Be(MessageStatus.Failed);
    }

    [Fact]
    public void AddReply_ShouldAddToRepliesCollection()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var senderId = UserId.Create(Guid.NewGuid());
        var message = Message.Create(conversationId, senderId, "Parent message");
        var reply = Message.CreateReply(
            conversationId, 
            senderId, 
            message.Id, 
            "Reply");

        // Act
        message.AddReply(reply);

        // Assert
        message.Replies.Should().Contain(reply);
    }
}