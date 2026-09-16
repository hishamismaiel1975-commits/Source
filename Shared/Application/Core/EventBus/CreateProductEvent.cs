using Platform.Lib.Core.EventBus;

namespace Application.Lib.Core.EventBus
{
    public class CreateProductEvent : IntegrationEvent
    {
        public required Guid ProductId { get; set; }
        public required string ProjectName { get; set; }
    }
}
