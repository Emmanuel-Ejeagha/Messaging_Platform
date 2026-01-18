using FluentValidation;

namespace MessagingPlatform.Application.Groups.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Group name is required")
            .MaximumLength(50).WithMessage("Group name cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner ID is required");

        RuleFor(x => x.MaxMembers)
            .GreaterThanOrEqualTo(2).WithMessage("Group must have at least 2 members")
            .LessThanOrEqualTo(1000).WithMessage("Group cannot have more than 1000 members");
    }
}