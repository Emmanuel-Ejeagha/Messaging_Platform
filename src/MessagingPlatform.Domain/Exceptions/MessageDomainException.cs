using System;

namespace MessagingPlatform.Domain.Exceptions;

    public class MessageDomainException : DomainException
    {
        public MessageDomainException(string message) : base("MESSAGE_ERROR", message) { }
    }
