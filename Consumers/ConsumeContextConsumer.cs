using MassTransit;
using OracleOutboxTest.Entities;
using OracleOutboxTest.Events;

namespace OracleOutboxTest.Consumers
{
    public class ConsumeContextConsumer(AppDbContext context, ILogger<ConsumeContextConsumer> logger) : IConsumer<ConsumeContextEvent>
    {
        public async Task Consume(ConsumeContext<ConsumeContextEvent> @event)
        {
            logger.LogInformation("ConsumeContextConsumer received event with Id: {Id}", @event.Message.Entity.Id);
            var entity = new ConsumeEntity
            {
                Id = Guid.NewGuid(),
                CorelationId = @event.Message.Entity.Id,
                CreatedDate = DateTime.UtcNow,
                Remark = @event.Message.Entity.Remark
            };

            logger.LogInformation("ConsumeContextConsumer is waiting for 5 seconds before saving changes to the database...");
            await Task.Delay(5000);

            await context.Consumes.AddAsync(entity, @event.CancellationToken);
            await context.SaveChangesAsync(@event.CancellationToken);
            logger.LogInformation("ConsumeContextConsumer has saved changes to the database for entity Id: {Id}", entity.Id);
        }
    }
}
