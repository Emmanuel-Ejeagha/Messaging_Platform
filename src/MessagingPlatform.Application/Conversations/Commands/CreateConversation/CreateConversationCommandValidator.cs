using FluentValidation;

namespace MessagingPlatform.Application.Conversations.Commands.CreateConversation;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.CreatorId)
            .NotEmpty().WithMessage("Creator ID is required");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid conversation type");

        When(x => x.Type == Domain.Enums.ConversationType.OneOnOne, () =>
        {
            RuleFor(x => x.ParticipantIds)
                .Must(ids => ids.Count == 2).WithMessage("One-on-one conversations must have exactly 2 participants")
                .Must(ids => ids.Distinct().Count() == 2).WithMessage("Participants must be unique");
        });

        When(x => x.Type == Domain.Enums.ConversationType.Group, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required for group conversations")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");
        });

        RuleForEach(x => x.ParticipantIds)
            .NotEmpty().WithMessage("Participant ID cannot be empty");

        RuleFor(x => x.ParticipantIds)
            .Must(ids => ids.Count <= 100).WithMessage("Cannot have more than 100 participants");
    }
}