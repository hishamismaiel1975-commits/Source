using Platform.Lib.Core.Persistence.Entities;

namespace Identity.Core.Persistence.Entities
{
    public class User : Entity
    {
        public required string UserName { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
        public required string Password { get; set; }
        public required bool IsBuiltIn { get; set; } = false;
    }

}
