using FluentValidation;

namespace MessagingPlatform.Application.Messages.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("Conversation ID is required");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required");

        RuleFor(x => x.Content)
            .MaximumLength(5000).WithMessage("Message content cannot exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Content));

        RuleFor(x => x.MediaUrl)
            .MaximumLength(2048).WithMessage("Media URL cannot exceed 2048 characters")
            .When(x => !string.IsNullOrEmpty(x.MediaUrl));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Content) || !string.IsNullOrEmpty(x.MediaUrl))
            .WithMessage("Message must have either content or media");

        RuleFor(x => x.ParentMessageId)
            .NotEqual(x => Guid.Empty).WithMessage("Parent message ID cannot be empty")
            .When(x => x.ParentMessageId.HasValue);
    }
}