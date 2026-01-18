using MediatR;
using Microsoft.AspNetCore.Mvc;
using MessagingPlatform.Application.Messages.Commands.SendMessage;
using MessagingPlatform.Application.Messages.Queries.GetConversationMessages;
using MessagingPlatform.Application.Messages.Queries.GetMessageById;
using MessagingPlatform.Application.Messages.Dtos;
using MessagingPlatform.Application.Messages.Commands.DeleteMessage;
using MessagingPlatform.Application.Messages.Commands.UpdateMessage;

namespace MessagingPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ISender _mediator;

    public MessagesController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="request">Message details</param>
    /// <param name="senderId">Sender ID (from header or auth context)</param>
    /// <returns>The sent message</returns>
    [HttpPost("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> SendMessage(
        Guid conversationId,
        [FromBody] SendMessageRequest request,
        [FromHeader(Name = "X-User-Id")] Guid senderId)
    {
        var command = new SendMessageCommand
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = request.Content,
            MediaUrl = request.MediaUrl,
            ParentMessageId = request.ParentMessageId
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMessage), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get messages from a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="includeReplies">Include message replies (default: false)</param>
    /// <returns>Paginated list of messages</returns>
    [HttpGet("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessagesResponse>> GetMessages(
        Guid conversationId,
        [FromHeader(Name = "X-User-Id")] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeReplies = false)
    {
        var query = new GetConversationMessagesQuery
        {
            ConversationId = conversationId,
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            IncludeReplies = includeReplies
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific message by ID
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <param name="userId">User ID (from header or auth context)</param>
    /// <returns>The message details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MessageDto>> GetMessage(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var query = new GetMessageByIdQuery
        {
            MessageId = id,
            UserId = userId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update a message
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <param name="request">Update details</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>The updated message</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> UpdateMessage(
        Guid id,
        [FromBody] UpdateMessageRequest request,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new UpdateMessageCommand
        {
            MessageId = id,
            UserId = userId,
            Content = request.Content
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a message (soft delete)
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <param name="userId">User ID (from header)</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage(
        Guid id,
        [FromHeader(Name = "X-User-Id")] Guid userId)
    {
        var command = new DeleteMessageCommand
        {
            MessageId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }
}