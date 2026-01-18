using MediatR;
using Microsoft.AspNetCore.Mvc;
using MessagingPlatform.Application.Conversations.Commands.CreateConversation;
using MessagingPlatform.Application.Conversations.Commands.UpdateConversation;
using MessagingPlatform.Application.Conversations.Commands.DeleteConversation;
using MessagingPlatform.Application.Conversations.Queries.GetConversationById;
using MessagingPlatform.Application.Conversations.Queries.GetConversations;
using MessagingPlatform.Application.Conversations.Dtos;

namespace MessagingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly ISender _mediator;

    public ConversationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get conversations for a user
    /// </summary>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <returns>Paginated list of conversations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations(
        [FromHeader(Name = "X-User-Id")] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetConversationsQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a conversation by ID
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <returns>The conversation details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ConversationDto>> GetConversation(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var query = new GetConversationByIdQuery
        {
            ConversationId = id,
            UserId = userId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    /// <param name="request">Conversation creation details</param>
    /// <param name="creatorId">Creator ID (from header or auth context)</param>
    /// <returns>The created conversation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ConversationDto>> CreateConversation(
        [FromBody] CreateConversationRequest request,
        [FromHeader(Name = "X-User-Id")] Guid creatorId)
    {
        var command = new CreateConversationCommand
        {
            Title = request.Title,
            Type = request.Type,
            ParticipantIds = request.ParticipantIds,
            GroupId = request.GroupId,
            CreatorId = creatorId
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetConversation), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update a conversation
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <param name="request">Update details</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>The updated conversation</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> UpdateConversation(
        Guid id,
        [FromBody] UpdateConversationRequest request,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new UpdateConversationCommand
        {
            ConversationId = id,
            UserId = userId,
            Title = request.Title,
            IsArchived = request.IsArchived
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Archive a conversation
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>The archived conversation</returns>
    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> ArchiveConversation(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new UpdateConversationCommand
        {
            ConversationId = id,
            UserId = userId,
            IsArchived = true
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Unarchive a conversation
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>The unarchived conversation</returns>
    [HttpPatch("{id:guid}/unarchive")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationDto>> UnarchiveConversation(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new UpdateConversationCommand
        {
            ConversationId = id,
            UserId = userId,
            IsArchived = false
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a conversation (soft delete)
    /// </summary>
    /// <param name="id">Conversation ID</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new DeleteConversationCommand
        {
            ConversationId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }
}