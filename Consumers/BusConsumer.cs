using MassTransit;
using MassTransit.Middleware;
using OracleOutboxTest.Entities;
using OracleOutboxTest.Events;

namespace OracleOutboxTest.Consumers
{
    public class BusConsumer(AppDbContext context, ILogger<BusConsumer> logger) : IConsumer<BusEvent>
    {
        public async Task Consume(ConsumeContext<BusEvent> @event)
        {
            logger.LogInformation("BusConsumer received event with Id: {Id}", @event.Message.Entity.Id);
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

            logger.LogInformation("BusConsumer is publishing ConsumeContextEvent with new PublishEntity Id: {Id}", publishEntity.Id);
            await InternalOutboxExtensions.SkipOutbox(@event).Publish(new ConsumeContextEvent(publishEntity, @event.Message.LongString));

            logger.LogInformation("BusConsumer is waiting for 5 seconds before saving changes to the database...");
            await Task.Delay(5000);

            await context.Consumes.AddAsync(consumeEntity, @event.CancellationToken);
            await context.Publishes.AddAsync(publishEntity, @event.CancellationToken);
            await context.SaveChangesAsync(@event.CancellationToken);
            logger.LogInformation("BusConsumer has saved changes to the database for entity Id: {Id}", publishEntity.Id);
        }
    }
}
