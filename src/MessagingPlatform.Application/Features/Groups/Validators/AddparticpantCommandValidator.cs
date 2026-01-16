using System;
using System.Data;
using FluentValidation;
using MessagingPlatform.Application.Features.Groups.Commands;

namespace MessagingPlatform.Application.Features.Groups.Validators;

public class AddparticpantCommandValidator
    : AbstractValidator<AddParticipantCommand>
{
    public AddparticpantCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.Role).IsInEnum();
    }
}
