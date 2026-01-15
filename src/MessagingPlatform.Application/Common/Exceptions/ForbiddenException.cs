using System;

namespace MessagingPlatform.Application.Common.Exceptions;

public class ForbiddenException :ApplicationException
{
    public ForbiddenException()
        : base("FORBIDDEN", "You do not have permission to perform this action")
    {        
    }
}
