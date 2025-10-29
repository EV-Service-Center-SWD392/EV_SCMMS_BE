using System.Security.Claims;
using EV_SCMMS.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace EV_SCMMS.WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            if (Guid.TryParse(sub, out var id)) return id;
            return null;
    }
  }

  public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
  public string? Role =>
       _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
}
