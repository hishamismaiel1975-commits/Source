using SequentialGuid;

namespace Platform.Core.Persistence.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; set; } = GuidV7.NewSqlGuid();
    }
}
