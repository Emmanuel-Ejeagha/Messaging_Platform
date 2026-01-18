using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Group : AggregateRoot
{
    private readonly List<Guid> _memberIds = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsPublic { get; private set; }
    public int MaxMembers { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<Guid> MemberIds => _memberIds.AsReadOnly();

    // Navigation property
    public Conversation? Conversation { get; private set; }

    // Private constructor for EF Core
    private Group() { }

    private Group(string name, UserId ownerId, string? description = null)
    {
        Name = name;
        OwnerId = ownerId.Value;
        Description = description;
        MaxMembers = 100; // Default max members
        IsPublic = false;

        _memberIds.Add(ownerId.Value);
    }

    public static Group Create(string name, UserId ownerId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("Group name cannot exceed 50 characters", nameof(name));

        return new Group(name, ownerId, description);
    }

    public void UpdateName(string newName, UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can update the group name");

        if (string.IsNullOrWhiteSpace(newName) || newName.Length > 50)
            throw new DomainException("Group name must be between 1 and 50 characters");

        Name = newName;
        SetUpdatedTimestamp();
    }

    public void UpdateDescription(string? description, UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can update the group description");

        if (description?.Length > 500)
            throw new DomainException("Description cannot exceed 500 characters");

        Description = description;
        SetUpdatedTimestamp();
    }

    public void AddMember(UserId userId)
    {
        if (_memberIds.Contains(userId.Value))
            throw new DomainException($"User {userId.Value} is already a member");

        if (_memberIds.Count >= MaxMembers)
            throw new DomainException($"Group has reached maximum capacity of {MaxMembers} members");

        _memberIds.Add(userId.Value);
        SetUpdatedTimestamp();
    }

    public void RemoveMember(UserId userId)
    {
        if (userId.Value == OwnerId)
            throw new DomainException("Cannot remove the group owner");

        if (!_memberIds.Contains(userId.Value))
            throw new DomainException($"User {userId.Value} is not a member");

        _memberIds.Remove(userId.Value);
        SetUpdatedTimestamp();
    }

    public void TransferOwnership(UserId newOwnerId, UserId currentOwnerId)
    {
        if (currentOwnerId.Value != OwnerId)
            throw new DomainException("Only the current owner can transfer ownership");

        if (!_memberIds.Contains(newOwnerId.Value))
            throw new DomainException("New owner must be a member of the group");

        OwnerId = newOwnerId.Value;
        SetUpdatedTimestamp();
    }

    public void SetMaxMembers(int maxMembers, UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can set maximum members");

        if (maxMembers < 2)
            throw new DomainException("Group must have at least 2 members");

        if (maxMembers < _memberIds.Count)
            throw new DomainException($"Cannot set maximum members below current member count ({_memberIds.Count})");

        MaxMembers = maxMembers;
        SetUpdatedTimestamp();
    }

    public void SetPublic(bool isPublic, UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can change group visibility");

        IsPublic = isPublic;
        SetUpdatedTimestamp();
    }

    public void UpdateAvatarUrl(string? avatarUrl, UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can update the avatar");

        if (avatarUrl?.Length > 2048)
            throw new DomainException("Avatar URL cannot exceed 2048 characters");

        AvatarUrl = avatarUrl;
        SetUpdatedTimestamp();
    }

    public bool IsMember(UserId userId)
    {
        return _memberIds.Contains(userId.Value);
    }

    public bool CanAddMember()
    {
        return _memberIds.Count < MaxMembers;
    }

    // EF Core needs access to set the member IDs
    private List<Guid> InternalMemberIds
    {
        get => _memberIds;
        set
        {
            _memberIds.Clear();
            if (value != null)
            {
                _memberIds.AddRange(value);
            }
        }
    }

    public void Delete(UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can delete the group");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        SetUpdatedTimestamp();
    }

    public void Restore(UserId requesterId)
    {
        if (requesterId.Value != OwnerId)
            throw new DomainException("Only the group owner can restore the group");

        IsDeleted = false;
        DeletedAt = null;
        SetUpdatedTimestamp();
    }
}