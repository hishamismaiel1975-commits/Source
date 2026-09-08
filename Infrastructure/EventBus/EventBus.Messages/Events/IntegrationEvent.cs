namespace EventBus.Messages.Events
{
    public class IntegrationEvent
    {
        public Guid CorrelationId { get; set; }
        public DateTime CreatedDate { get; set; }

        public IntegrationEvent()
        {
            CorrelationId = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid Id, DateTime CreatedDate)
        {
            this.CorrelationId = Id;
            this.CreatedDate = CreatedDate;
        }
    }
}
