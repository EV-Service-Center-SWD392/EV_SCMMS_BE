using Microsoft.AspNetCore.Authorization;

namespace EV_SCMMS.WebAPI.Authorization;

/// <summary>
/// Authorization requirement to validate that access token exists in refresh tokens
/// </summary>
public class ValidRefreshTokenRequirement : IAuthorizationRequirement
{
    public ValidRefreshTokenRequirement()
    {
    }
}