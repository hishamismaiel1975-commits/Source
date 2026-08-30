namespace EventBus.Messages.Events
{
    public class IntegrationEvent
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid Id, DateTime CreatedDate)
        {
            this.Id = Id;
            this.CreatedDate = CreatedDate;
        }
    }
}
