namespace Application.Lib.Core.Services.Identity.DTOs
{
    public record UserPermissionsResponse
    (
         ICollection<string> Permissions
     );
}
