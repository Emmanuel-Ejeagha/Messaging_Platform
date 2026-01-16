using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Queries;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class GetConversationParticipantsQueryValidator : AbstractValidator<GetConversationParticipantsQuery>
{
    public GetConversationParticipantsQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
    }
}