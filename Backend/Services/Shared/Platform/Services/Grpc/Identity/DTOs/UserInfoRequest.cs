namespace Platform.Lib.Services.Grpc.Identity.DTOs
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
