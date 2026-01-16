using System;
using System.Security.Claims;
using MessagingPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MessagingPlatform.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Guid UserId
    {
        get
        {
            var UserIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (UserIdClaim != null && Guid.TryParse(UserIdClaim.Value, out var userId))
            {
                return userId;
            }

            // For development/testing allow header override
            var userIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userIdHeader) && Guid.TryParse(userIdHeader, out var headerUserId))
            {
                return headerUserId;
            }

            throw new UnauthorizedAccessException("User ID not found in claims");
        }
    }

    public bool IsAuthenthicated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
}

