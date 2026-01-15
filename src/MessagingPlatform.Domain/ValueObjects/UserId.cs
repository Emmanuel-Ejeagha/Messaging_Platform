using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Exceptions;

namespace MessagingPlatform.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    public Guid Value { get; }
    private UserId(Guid value) => Value = value;
    public static UserId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("User ID cannot be empty");

        return new UserId(value);
    }

    public static UserId New() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => From(value);
}
