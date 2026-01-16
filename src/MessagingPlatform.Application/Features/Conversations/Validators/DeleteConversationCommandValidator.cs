using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Commands;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
    public DeleteConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
    }
}
