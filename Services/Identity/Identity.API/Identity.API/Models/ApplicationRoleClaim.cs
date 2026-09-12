using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
    }

}

