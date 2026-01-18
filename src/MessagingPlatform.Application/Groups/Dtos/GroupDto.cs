namespace MessagingPlatform.Application.Groups.Dtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsPublic { get; set; }
    public int MaxMembers { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
}

public class CreateGroupRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public int MaxMembers { get; set; } = 100;
}

public class UpdateGroupRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public int? MaxMembers { get; set; }
    public string? AvatarUrl { get; set; }
}

public class AddGroupMemberRequest
{
    public Guid UserId { get; set; }
}

public class DeleteGroupRequest
{
}