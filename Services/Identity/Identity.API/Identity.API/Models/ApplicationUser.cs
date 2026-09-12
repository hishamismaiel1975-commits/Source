using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullNameEn { get; set; }
        public required string FullNameAr { get; set; }

    }

}