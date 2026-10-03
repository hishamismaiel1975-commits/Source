using Platform.Lib.Services.Identity.Enums;

namespace Platform.Lib.Services.Security
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? NameEn { get; }
        string? NameAr { get; }
        UserTypes? UserType { get; }
        string? UserTypeName { get; }
        IReadOnlyCollection<string> Permissions { get; }
        bool HasPermission(string permission);
        bool IsAuthenticated { get; }
    }
}
