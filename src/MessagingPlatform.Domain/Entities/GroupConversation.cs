using System;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Events;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;
using static MessagingPlatform.Domain.Exceptions.DomainException;

namespace MessagingPlatform.Domain.Entities;

public sealed class GroupConversation : Conversation
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? AvatarUrl { get; private set; }

    public override ConversationType Type => ConversationType.Group;

    public GroupConversation(string name)
    {
        UpdateName(name);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Group name cannot be empty");

        if (name.Length > 100)
            throw new DomainException("Group name cannot exceed 100 characters");

        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        if (description?.Length > 500)
            throw new DomainException("Description cannot exceed 500 characters");

        Description = description;
        UpdateTimestamp();
    }

    public void UpdateAvatar(string? avatarUrl)
    {
        // Validate URL format if provided
        if (!string.IsNullOrWhiteSpace(avatarUrl) && !Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
            throw new DomainException("Invalid avatar URL format");

        AvatarUrl = avatarUrl;
        UpdateTimestamp();
    }

    public void ChangeParticipantRole(UserId userId, ParticipantRole newRole)
    {
        var participant = Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
            throw new ParticipantDomainException("Participant not found");

        participant.ChangeRole(newRole);
        UpdateTimestamp();

        AddDomainEvent(new ParticipantRoleChangedEvent(Id, userId, newRole));
    }

}
