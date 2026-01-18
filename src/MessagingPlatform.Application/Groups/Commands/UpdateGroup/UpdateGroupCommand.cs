using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Groups.Dtos;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand : IRequest<GroupDto>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public bool? IsPublic { get; init; }
    public int? MaxMembers { get; init; }
    public string? AvatarUrl { get; init; }
}

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, GroupDto>
{
    private readonly IGroupRepository _groupRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupDto> Handle(
        UpdateGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);

        if (group == null)
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId);

        var userId = UserId.Create(request.UserId);

        // Update properties if provided
        if (request.Name != null)
            group.UpdateName(request.Name, userId);

        if (request.Description != null)
            group.UpdateDescription(request.Description, userId);

        if (request.IsPublic.HasValue)
            group.SetPublic(request.IsPublic.Value, userId);

        if (request.MaxMembers.HasValue)
            group.SetMaxMembers(request.MaxMembers.Value, userId);

        if (request.AvatarUrl != null)
            group.UpdateAvatarUrl(request.AvatarUrl, userId);

        _groupRepository.Update(group);

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