namespace Platform.Lib.EventBus
{
    public class CreateProductEvent : IntegrationEvent
    {
        public required Guid ProductId { get; set; }
        public required string ProjectName { get; set; }
    }
}
