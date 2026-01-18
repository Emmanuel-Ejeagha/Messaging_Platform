using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Exceptions;

namespace MessagingPlatform.Domain.ValueObjects;

public sealed class MessageContent : ValueObject
{
    public string Content { get; }
    public string? MediaUrl { get; }
    public bool HasMedia => !string.IsNullOrEmpty(MediaUrl);

    private const int MaxLength = 5000;
    private const int MaxMediaUrlLength = 2048;

    private MessageContent(string content, string? mediaUrl = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(mediaUrl))
            throw new DomainException("Message must have either content or media");

        if (content?.Length > MaxLength)
            throw new DomainException($"Message content cannot exceed {MaxLength} characters");

        if (mediaUrl?.Length > MaxMediaUrlLength)
            throw new DomainException($"Media URL cannot exceed {MaxMediaUrlLength} characters");

        Content = content ?? string.Empty;
        MediaUrl = mediaUrl;
    }

    public static MessageContent Create(string content, string? mediaUrl = null)
    {
        return new MessageContent(content, mediaUrl);
    }

    public static MessageContent CreateWithMedia(string mediaUrl, string? caption = null)
    {
        return new MessageContent(caption ?? string.Empty, mediaUrl);
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(MediaUrl);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Content;
        yield return MediaUrl ?? string.Empty;
    }

    public override string ToString() => Content;
}