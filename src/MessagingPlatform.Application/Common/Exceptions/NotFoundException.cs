using System;

namespace MessagingPlatform.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string name, object key)
        : base("NOT_FOUND", $"{name} with id {key} was not found")
    {
        
    }
}
