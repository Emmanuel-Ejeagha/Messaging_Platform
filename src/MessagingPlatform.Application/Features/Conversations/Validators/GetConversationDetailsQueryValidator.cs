using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Queries;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class GetConversationDetailsQueryValidator : AbstractValidator<GetConversationDetailsQuery>
{
    public GetConversationDetailsQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
        
        RuleFor(x => x.MessageLimit)
            .InclusiveBetween(1, 200)
            .WithMessage("Message limit must be between 1 and 200");
    }
}
