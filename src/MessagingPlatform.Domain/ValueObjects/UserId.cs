using MessagingPlatform.Domain.Common;

namespace MessagingPlatform.Domain.ValueObjects;

public sealed class UserId : ValueObject
{
    public Guid Value { get; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(value));

        return new UserId(value);
    }

    public static UserId CreateUnique()
    {
        return new UserId(Guid.NewGuid());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}