using System;

namespace MessagingPlatform.Application.Common.Interfaces;

public class ICurrentUserService
{
    public Guid UserId { get; }
    public bool IsAuthenticated { get; }
    public string? Email { get; }
    public string? Username { get; }
}
