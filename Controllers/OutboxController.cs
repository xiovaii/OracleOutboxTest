using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OracleOutboxTest.Entities;
using OracleOutboxTest.Events;

namespace OracleOutboxTest.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OutboxController(AppDbContext context, IPublishEndpoint publishEndpoint, IBus bus, ILogger<OutboxController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<PublishEntity> PublishEndpoint(string? remark, string? longString)
        {
            var entity = new PublishEntity
            {
                Id = Guid.NewGuid(),
                Remark = remark,
                CreatedDate = DateTime.UtcNow
            };

            await context.Publishes.AddAsync(entity);
            await publishEndpoint.Publish(new PublishEndpointEvent(entity, longString));
            logger.LogInformation("Published event with Id: {Id}", entity.Id);

            logger.LogInformation("Waiting for 5 seconds before saving changes to the database...");
            await Task.Delay(5000);
            await context.SaveChangesAsync();
            logger.LogInformation("Changes saved to the database for entity Id: {Id}", entity.Id);


            return entity;
        }

        [HttpPost]
        public async Task<PublishEntity> Bus(string? remark, string? longString)
        {
            var entity = new PublishEntity
            {
                Id = Guid.NewGuid(),
                Remark = remark,
                CreatedDate = DateTime.UtcNow
            };

            await context.Publishes.AddAsync(entity);
            await bus.Publish(new BusEvent(entity, longString));
            logger.LogInformation("Published event with Id: {Id}", entity.Id);

            logger.LogInformation("Waiting for 5 seconds before saving changes to the database...");
            await Task.Delay(5000);
            await context.SaveChangesAsync();
            logger.LogInformation("Changes saved to the database for entity Id: {Id}", entity.Id);

            return entity;
        }
    }
}
