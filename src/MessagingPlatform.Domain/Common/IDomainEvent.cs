using MediatR;

namespace MessagingPlatform.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}