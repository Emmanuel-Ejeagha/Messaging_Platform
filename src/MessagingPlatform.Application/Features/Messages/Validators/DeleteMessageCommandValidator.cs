using System;
using FluentValidation;
using MessagingPlatform.Application.Features.Messages.Commands;

namespace MessagingPlatform.Application.Features.Messages.Validators;

public class DeleteMessageCommandValidator
    : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(d => d.MessageId).NotEmpty();
    }
}
