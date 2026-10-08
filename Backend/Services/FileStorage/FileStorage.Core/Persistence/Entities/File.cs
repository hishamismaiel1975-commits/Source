using Platform.Lib.Persistence.Entities;

namespace FileStorage.Core.Persistence.Entities
{
    public class File : Entity
    {
        public required string Path { get; set; }
        public required string Type { get; set; }
        public required bool IsCustomer { get; set; }
        public Guid? CustomerId { get; set; }

    }
}
