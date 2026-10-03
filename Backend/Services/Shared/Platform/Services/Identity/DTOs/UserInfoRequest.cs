namespace Platform.Lib.Services.Identity.DTOs
{
    public record UserInfoRequest
    (
        string NameEn,
        string NameAr,
        string UserType,
        bool IsActive,
        ICollection<string> Permissions
     );
}
