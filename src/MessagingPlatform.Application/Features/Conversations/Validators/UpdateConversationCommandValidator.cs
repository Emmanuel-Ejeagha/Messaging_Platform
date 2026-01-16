using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Commands;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class UpdateConversationCommandValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
        
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters");
        });
        
        When(x => !string.IsNullOrEmpty(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters");
        });
        
        When(x => !string.IsNullOrEmpty(x.AvatarUrl), () =>
        {
            RuleFor(x => x.AvatarUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Avatar URL must be a valid URL");
        });
    }
}