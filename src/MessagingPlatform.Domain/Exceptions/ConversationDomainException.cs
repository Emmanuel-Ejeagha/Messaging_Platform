using System;

namespace MessagingPlatform.Domain.Exceptions;

public class ConversationDomainException : DomainException
{
    public ConversationDomainException(string message) : base("CONVERSATION_ERROR", message) { }
}
