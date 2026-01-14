using System;

namespace MessagingPlatform.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }
    public DomainException(string message) : base(message)
    {
        Code = "DOMAIN_ERROR";
    }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
        Code = "DOMAIN_ERROR";
    }

    public class ConversationDomainException : DomainException
    {
        public ConversationDomainException(string message) : base("CONVERSATION_ERROR", message) { }
    }

    
}
