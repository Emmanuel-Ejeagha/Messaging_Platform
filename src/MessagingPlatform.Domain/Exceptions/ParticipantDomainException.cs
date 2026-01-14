using System;

namespace MessagingPlatform.Domain.Exceptions;


    public class ParticipantDomainException : DomainException
    {
        public ParticipantDomainException(string message) : base("PARTICIPATION_ERROR", message){}
    }
