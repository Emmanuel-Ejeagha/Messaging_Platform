using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Commands;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class ArchiveConversationCommandValidator : AbstractValidator<ArchiveConversationCommand>
{
    public ArchiveConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
    }
}
