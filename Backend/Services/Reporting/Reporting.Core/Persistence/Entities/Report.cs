using Platform.Lib.Persistence.Entities;

namespace Reporting.Core.Persistence.Entities
{
    public class Report : Entity
    {
        public string Name { get; set; }
        public string Title { get; set; }

    }
}
