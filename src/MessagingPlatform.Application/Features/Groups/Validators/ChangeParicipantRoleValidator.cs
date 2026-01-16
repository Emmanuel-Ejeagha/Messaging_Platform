using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Groups.Commands;

namespace MessagingPlatform.Application.Features.Groups.Validators;

public class ChangeParicipantRoleValidator : AbstractValidator<ChangeParticipantRoleCommand>
{
    public ChangeParicipantRoleValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.NewRole).IsInEnum();
    }
}
