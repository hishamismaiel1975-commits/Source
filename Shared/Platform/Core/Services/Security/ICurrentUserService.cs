using Platform.Lib.Core.Services.Identity.Enums;

namespace Platform.Lib.Core.Services.Security
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? NameEn { get; }
        string? NameAr { get; }
        UserTypes? UserType { get; }
        IReadOnlyCollection<string> Permissions { get; }
        bool HasPermission(string permission);
        bool IsAuthenticated { get; }
    }
}
