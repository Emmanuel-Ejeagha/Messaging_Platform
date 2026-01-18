using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Groups.Dtos;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Groups.Commands.CreateGroup;

public record CreateGroupCommand : IRequest<GroupDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsPublic { get; init; } = false;
    public int MaxMembers { get; init; } = 100;
    public Guid OwnerId { get; init; }
}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupDto>
{
    private readonly IGroupRepository _groupRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupDto> Handle(
        CreateGroupCommand request,
        CancellationToken cancellationToken)
    {
        // Create group
        var group = Group.Create(
            request.Name,
            UserId.Create(request.OwnerId),
            request.Description);

        // Set additional properties
        group.SetPublic(request.IsPublic, UserId.Create(request.OwnerId));
        group.SetMaxMembers(request.MaxMembers, UserId.Create(request.OwnerId));

        // Save group
        await _groupRepository.AddAsync(group, cancellationToken);

        return MapToDto(group);
    }

    private static GroupDto MapToDto(Domain.Entities.Group group)
    {
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            OwnerId = group.OwnerId,
            AvatarUrl = group.AvatarUrl,
            IsPublic = group.IsPublic,
            MaxMembers = group.MaxMembers,
            MemberCount = group.MemberIds.Count,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt,
            MemberIds = group.MemberIds.ToList()
        };
    }
}