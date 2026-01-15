using System;

namespace MessagingPlatform.Application.Common.Exceptions;

public class ApplicationException : Exception
{
    public string Code { get; }

    public ApplicationException(string code, string message) : base(message)
    {
        Code = code;
    }
}
