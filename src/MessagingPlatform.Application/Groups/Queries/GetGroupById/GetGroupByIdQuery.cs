using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Groups.Dtos;

namespace MessagingPlatform.Application.Groups.Queries.GetGroupById;

public record GetGroupByIdQuery : IRequest<GroupDto>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDto>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupByIdQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupDto> Handle(
        GetGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);

        if (group == null)
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId);

        // Check if user is a member (unless group is public)
        if (!group.IsPublic && !group.IsMember(Domain.ValueObjects.UserId.Create(request.UserId)))
            throw new ForbiddenException("You are not a member of this group");

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