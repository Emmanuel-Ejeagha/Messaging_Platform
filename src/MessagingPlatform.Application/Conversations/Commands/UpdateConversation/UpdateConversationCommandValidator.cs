using FluentValidation;

namespace MessagingPlatform.Application.Conversations.Commands.UpdateConversation;

public class UpdateConversationCommandValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("Conversation ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty if provided")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");
        });
    }
}