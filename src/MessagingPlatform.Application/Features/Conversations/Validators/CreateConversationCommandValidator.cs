using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Conversations.Commands;
using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Features.Conversations.Validators;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid conversation type");

        When(x => x.Type == ConversationType.OneToOne, () =>
        {
            RuleFor(x => x.OtherUserId)
                .NotNull()
                .WithMessage("OtherUserId is required for one-to-one conversationa")
                .NotEqual(Guid.Empty)
                .WithMessage("OtherUserId cannot be empty");

            RuleFor(x => x.Name)
                .Null()
                .WithMessage("Name is not allowed for one-to-one conversations");
        });

        When(x => x.Type == ConversationType.Group, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Group name is required")
                .MaximumLength(100)
                .WithMessage("Group name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.ParticipantIds)
                .Must(ids => ids == null || ids.All(id => id != Guid.Empty))
                .WithMessage("Invalid participant ID found");
        });
    }
}
