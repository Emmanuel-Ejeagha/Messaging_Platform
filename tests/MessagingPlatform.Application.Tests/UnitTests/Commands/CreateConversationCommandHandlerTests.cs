using FluentAssertions;
using Moq;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Conversations.Commands.CreateConversation;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;
using Xunit;

namespace MessagingPlatform.Application.Tests.UnitTests.Commands;

public class CreateConversationCommandHandlerTests
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly CreateConversationCommandHandler _handler;

    public CreateConversationCommandHandlerTests()
    {
        _conversationRepositoryMock = new Mock<IConversationRepository>();
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _handler = new CreateConversationCommandHandler(
            _conversationRepositoryMock.Object,
            _groupRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidOneOnOneCommand_ShouldCreateConversation()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            Title = "Chat",
            Type = ConversationType.OneOnOne,
            ParticipantIds = new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000002")
            },
            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        _conversationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ConversationType.OneOnOne);
        result.Participants.Should().HaveCount(2);
        
        _conversationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidGroupCommand_ShouldCreateConversation()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            Title = "Group Chat",
            Type = ConversationType.Group,
            ParticipantIds = new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            },
            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        _conversationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ConversationType.Group);
        result.Title.Should().Be("Group Chat");
        
        _conversationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_OneOnOneWithWrongParticipantCount_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            Title = "Chat",
            Type = ConversationType.OneOnOne,
            ParticipantIds = new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            },
            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GroupWithoutTitle_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            Title = "",
            Type = ConversationType.Group,
            ParticipantIds = new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            },
            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentGroup_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateConversationCommand
        {
            Title = "Group Chat",
            Type = ConversationType.Group,
            ParticipantIds = new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            },
            GroupId = Guid.Parse("00000000-0000-0000-0000-000000000999"),
            CreatorId = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        _groupRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupId.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}