using MassTransit;
using MassTransit.Middleware;
using OracleOutboxTest.Entities;
using OracleOutboxTest.Events;

namespace OracleOutboxTest.Consumers
{
    public class PublishEndpointConsumer(AppDbContext context, ILogger<PublishEndpointConsumer> logger) : IConsumer<PublishEndpointEvent>
    {
        public async Task Consume(ConsumeContext<PublishEndpointEvent> @event)
        {
            logger.LogInformation("PublishEndpointConsumer received event with Id: {Id}", @event.Message.Entity.Id);
            var consumeEntity = new ConsumeEntity
            {
                Id = Guid.NewGuid(),
                CorelationId = @event.Message.Entity.Id,
                CreatedDate = DateTime.UtcNow,
                Remark = nameof(PublishEndpointConsumer)
            };

            var publishEntity = new PublishEntity
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Remark = $"{nameof(PublishEndpointConsumer)} to {nameof(ConsumeContextEvent)}"
            };

            logger.LogInformation("PublishEndpointConsumer is publishing ConsumeContextEvent with new PublishEntity Id: {Id}", publishEntity.Id);
            await InternalOutboxExtensions.SkipOutbox(@event).Publish(new ConsumeContextEvent(publishEntity, @event.Message.LongString));

            logger.LogInformation("PublishEndpointConsumer is waiting for 5 seconds before saving changes to the database...");
            await Task.Delay(5000);

            await context.Consumes.AddAsync(consumeEntity, @event.CancellationToken);
            await context.Publishes.AddAsync(publishEntity, @event.CancellationToken);
            await context.SaveChangesAsync(@event.CancellationToken);
            logger.LogInformation("PublishEndpointConsumer has saved changes to the database for entity Id: {Id}", publishEntity.Id);
        }
    }
}
