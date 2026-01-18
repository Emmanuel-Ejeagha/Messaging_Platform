using MediatR;
using Microsoft.AspNetCore.Mvc;
using MessagingPlatform.Application.Groups.Commands.CreateGroup;
using MessagingPlatform.Application.Groups.Queries.GetGroupById;
using MessagingPlatform.Application.Groups.Queries.GetUserGroups;
using MessagingPlatform.Application.Groups.Dtos;
using MessagingPlatform.Application.Groups.Commands.DeleteGroup;
using MessagingPlatform.Application.Groups.Commands.UpdateGroup;

namespace MessagingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly ISender _mediator;

    public GroupsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    /// <param name="request">Group creation details</param>
    /// <param name="ownerId">Owner ID (from header or auth context)</param>
    /// <returns>The created group</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupDto>> CreateGroup(
        [FromBody] CreateGroupRequest request,
        [FromHeader(Name = "X-User-Id")] Guid ownerId)
    {
        var command = new CreateGroupCommand
        {
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            MaxMembers = request.MaxMembers,
            OwnerId = ownerId
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetGroup), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a group by ID
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <returns>The group details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GroupDto>> GetGroup(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var query = new GetGroupByIdQuery
        {
            GroupId = id,
            UserId = userId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get groups for a user
    /// </summary>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Paginated list of groups</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GroupDto>>> GetGroups(
        [FromHeader(Name = "X-User-Id")] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetUserGroupsQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
    /// <summary>
    /// Update a group
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="request">Update details</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>The updated group</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDto>> UpdateGroup(
        Guid id,
        [FromBody] UpdateGroupRequest request,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new UpdateGroupCommand
        {
            GroupId = id,
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            MaxMembers = request.MaxMembers,
            AvatarUrl = request.AvatarUrl
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a group (soft delete)
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroup(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new DeleteGroupCommand
        {
            GroupId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }
}