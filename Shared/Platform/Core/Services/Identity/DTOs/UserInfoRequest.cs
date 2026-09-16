namespace Platform.Lib.Core.Services.Identity.DTOs
{
    public record UserInfoRequest
    (
        string NameEn,
        string NameAr,
        string UserType,
        ICollection<string> Permissions
     );
}
