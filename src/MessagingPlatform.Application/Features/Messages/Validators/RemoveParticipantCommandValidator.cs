using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Messages.Commands;

namespace MessagingPlatform.Application.Features.Messages.Validators;

public class RemoveParticipantCommandValidator : AbstractValidator<RemoveParticipantCommand>
{
    public RemoveParticipantCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
        
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}