using SequentialGuid;

namespace Platform.Lib.Core.Persistence.Entities
{
    public abstract class Entity
    {
        // Use GuidV7.NewSqlGuid() to generate a sequential GUID for better performance
        // To can copy data from one database to another without changing the GUIDs
        public Guid Id { get; set; } = GuidV7.NewSqlGuid();
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
