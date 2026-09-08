using Core.EventBus.Events;
using MassTransit;
using System.Text.Json;

namespace Discount.API.EventBus.Consumer
{
    public class CreateProductConsumer : IConsumer<CreateProductEvent>
    {
        private ILogger<CreateProductConsumer> _logger { get; set; }
        public CreateProductConsumer(ILogger<CreateProductConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<CreateProductEvent> context)
        {
            var message = context.Message;

            // Handle CreateProductEvent
            _logger.LogInformation("Handling CreateProductEvent: {message}", JsonSerializer.Serialize(message));
        }
    }
}
