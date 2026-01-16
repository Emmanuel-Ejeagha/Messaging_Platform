using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Messages.Commands;

namespace MessagingPlatform.Application.Features.Messages.Validators;

public class MarkMessagesAsReadCommandValidator : AbstractValidator<MarkMessagesAsReadCommand>
{
    public MarkMessagesAsReadCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
        
        RuleFor(x => x.MessageIds)
            .NotEmpty()
            .WithMessage("At least one message ID is required")
            .Must(ids => ids.Count <= 100)
            .WithMessage("Cannot mark more than 100 messages as read at once");
        
        RuleForEach(x => x.MessageIds)
            .NotEmpty()
            .WithMessage("Message ID cannot be empty");
    }
}
