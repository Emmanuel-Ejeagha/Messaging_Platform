using System.Text.Json;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;

namespace MessagingPlatform.Domain.ValueObjects;

public sealed class MessageContent : ValueObject
{
    public string Text { get; }
    public MessageContentType Type { get; }
    public Dictionary<string, object>? Metadata { get; private set; }
    
    private MessageContent(string text, MessageContentType type)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException("Message text cannot be empty");
        
        if (type != MessageContentType.Deleted && text.Length > 5000)
            throw new DomainException("Message text cannot exceed 5000 characters");
        
        Text = text.Trim();
        Type = type;
    }
    
    public static MessageContent CreateText(string text) => new(text, MessageContentType.Text);
    public static MessageContent CreateSystem(string text) => new(text, MessageContentType.System);
    public static MessageContent CreateDeleted(string text) => new(text, MessageContentType.Deleted);
    
    public void AddMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Text;
        yield return Type;
        if (Metadata != null)
        {
            yield return JsonSerializer.Serialize(Metadata);
        }
    }
}