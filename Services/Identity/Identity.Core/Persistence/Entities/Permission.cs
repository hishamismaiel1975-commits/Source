using Platform.Lib.Core.Persistence.Entities;

namespace Identity.Core.Persistence.Entities
{
    public class Permission : Entity
    {
        public required string Name { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    }

}
