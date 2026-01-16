using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Messages.Commands;

namespace MessagingPlatform.Application.Features.Messages.Validators;

public class EditMessageCommandValidator :  AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
