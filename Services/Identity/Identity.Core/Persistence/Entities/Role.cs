using Platform.Lib.Core.Persistence.Entities;

namespace Identity.Core.Persistence.Entities
{
    public class Role : Entity
    {
        public required string Name { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
        public required bool IsBuiltIn { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}