using FluentValidation;

namespace MessagingPlatform.Application.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty if provided")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        });

        When(x => x.MaxMembers.HasValue, () =>
        {
            RuleFor(x => x.MaxMembers)
                .GreaterThanOrEqualTo(2).WithMessage("Group must have at least 2 members")
                .LessThanOrEqualTo(1000).WithMessage("Group cannot have more than 1000 members");
        });

        When(x => x.AvatarUrl != null, () =>
        {
            RuleFor(x => x.AvatarUrl)
                .MaximumLength(2048).WithMessage("Avatar URL cannot exceed 2048 characters");
        });
    }
}