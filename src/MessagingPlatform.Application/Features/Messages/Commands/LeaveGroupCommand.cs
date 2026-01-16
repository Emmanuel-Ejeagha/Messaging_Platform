using System;
using FluentValidation;
using MediatR;
using MessagingPlatform.Application.Common.Result;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class LeaveGroupCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
}

public class LeaveGroupCommandValidator : AbstractValidator<LeaveGroupCommand>
{
    public LeaveGroupCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
    }
}
