namespace Platform.Lib.Core.Services.Identity.DTOs
{
    public record UserPermissionsResponse
    (
         ICollection<string> Permissions
     );
}
