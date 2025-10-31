namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }

   string? Role { get; }
}
