using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Messages.Queries;

namespace MessagingPlatform.Application.Features.Messages.Validators;

public class GetMessageQueryValidator : AbstractValidator<GetMessageQuery>
{
    public GetMessageQueryValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
        
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("Message ID is required");
    }
}
