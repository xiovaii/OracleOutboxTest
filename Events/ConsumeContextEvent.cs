using OracleOutboxTest.Entities;

namespace OracleOutboxTest.Events
{
    public record ConsumeContextEvent(PublishEntity Entity, string? LongString);
}
