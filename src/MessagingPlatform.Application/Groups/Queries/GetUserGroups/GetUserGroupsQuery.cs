using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Groups.Dtos;

namespace MessagingPlatform.Application.Groups.Queries.GetUserGroups;

public record GetUserGroupsQuery : IRequest<List<GroupDto>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, List<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;

    public GetUserGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<List<GroupDto>> Handle(
        GetUserGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetByMemberIdAsync(
            request.UserId,
            cancellationToken);

        // Simple pagination (in a real app, you'd implement proper pagination in the repository)
        var paginatedGroups = groups
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paginatedGroups.Select(MapToDto).ToList();
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