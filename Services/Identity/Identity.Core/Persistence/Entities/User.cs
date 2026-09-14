using Identity.Core.Enums;
using Platform.Lib.Core.Persistence.Entities;

namespace Identity.Core.Persistence.Entities
{
    public class User : Entity
    {
        public required string UserName { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
        public required string PasswordHash { get; set; }
        public UserType UserType { get; set; }
        public Guid? RoleId { get; set; }
        public bool IsActive { get; set; }
        public required bool IsBuiltIn { get; set; }

        public Role? Role { get; set; }
    }

}
