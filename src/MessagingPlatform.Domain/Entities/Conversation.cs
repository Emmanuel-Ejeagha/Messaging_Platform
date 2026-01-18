using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Events;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Conversation : AggregateRoot
{
    private readonly List<Participant> _participants = new();
    private readonly List<Guid> _messageIds = new();

    public string Title { get; private set; } = string.Empty;
    public ConversationType Type { get; private set; }
    public Guid? GroupId { get; private set; }
    public DateTime LastMessageAt { get; private set; }
    public bool IsArchived { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();
    public IReadOnlyCollection<Guid> MessageIds => _messageIds.AsReadOnly();

    // Private constructor for EF Core
    private Conversation() { }

    private Conversation(string title, ConversationType type, Guid? groupId = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Type = type;
        GroupId = groupId;
        LastMessageAt = CreatedAt;
        IsArchived = false;
    }

    public static Conversation CreateOneOnOne(UserId user1, UserId user2)
    {
        var conversation = new Conversation($"Chat: {user1.Value} & {user2.Value}", ConversationType.OneOnOne);
        
        conversation.AddParticipant(user1, ParticipantRole.Member);
        conversation.AddParticipant(user2, ParticipantRole.Member);

        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Title,
            new List<Guid> { user1.Value, user2.Value }));

        return conversation;
    }

    public static Conversation CreateGroup(string title, UserId creatorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Group title cannot be empty", nameof(title));

        if (title.Length > 100)
            throw new ArgumentException("Group title cannot exceed 100 characters", nameof(title));

        var conversation = new Conversation(title, ConversationType.Group);
        conversation.AddParticipant(creatorId, ParticipantRole.Owner);

        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Title,
            new List<Guid> { creatorId.Value }));

        return conversation;
    }

    public void AddParticipant(UserId userId, ParticipantRole role = ParticipantRole.Member)
    {
        if (_participants.Any(p => p.UserId == userId.Value))
            throw new DomainException($"User {userId.Value} is already a participant in this conversation");

        if (Type == ConversationType.OneOnOne && _participants.Count >= 2)
            throw new DomainException("One-on-one conversations cannot have more than 2 participants");

        var participant = Participant.Create(userId, role, Id);
        _participants.Add(participant);
        
        SetUpdatedTimestamp();
    }

    public void RemoveParticipant(UserId userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId.Value);
        if (participant == null)
            throw new DomainException($"User {userId.Value} is not a participant in this conversation");

        // Don't allow removing the last participant
        if (_participants.Count <= 1)
            throw new DomainException("Cannot remove the last participant from a conversation");

        _participants.Remove(participant);
        SetUpdatedTimestamp();
    }

    public void UpdateTitle(string newTitle, UserId requesterId)
    {
        if (Type != ConversationType.Group)
            throw new DomainException("Only group conversations can have their title updated");

        var requester = _participants.FirstOrDefault(p => p.UserId == requesterId.Value);
        if (requester == null || requester.Role == ParticipantRole.Member)
            throw new DomainException("Only admins and owners can update conversation title");

        if (string.IsNullOrWhiteSpace(newTitle) || newTitle.Length > 100)
            throw new DomainException("Title must be between 1 and 100 characters");

        Title = newTitle;
        SetUpdatedTimestamp();
    }

    public void AddMessage(Guid messageId)
    {
        if (_messageIds.Contains(messageId))
            throw new DomainException($"Message {messageId} is already in this conversation");

        _messageIds.Add(messageId);
        LastMessageAt = DateTime.UtcNow;
        SetUpdatedTimestamp();
    }

    public void Archive()
    {
        IsArchived = true;
        SetUpdatedTimestamp();
    }

    public void Unarchive()
    {
        IsArchived = false;
        SetUpdatedTimestamp();
    }

    public bool HasParticipant(UserId userId)
    {
        return _participants.Any(p => p.UserId == userId.Value);
    }

    public Participant? GetParticipant(UserId userId)
    {
        return _participants.FirstOrDefault(p => p.UserId == userId.Value);
    }
    public void Delete(UserId requesterId)
{
    var requester = _participants.FirstOrDefault(p => p.UserId == requesterId.Value);
    if (requester == null || (requester.Role != ParticipantRole.Admin && requester.Role != ParticipantRole.Owner))
        throw new DomainException("Only admins and owners can delete conversations");

    IsDeleted = true;
    DeletedAt = DateTime.UtcNow;
    SetUpdatedTimestamp();
}

    public void Restore(UserId requesterId)
    {
        var requester = _participants.FirstOrDefault(p => p.UserId == requesterId.Value);
        if (requester == null || (requester.Role != ParticipantRole.Admin && requester.Role != ParticipantRole.Owner))
            throw new DomainException("Only admins and owners can restore conversations");

        IsDeleted = false;
        DeletedAt = null;
        SetUpdatedTimestamp();
    }
}