using OracleOutboxTest.Entities;

namespace OracleOutboxTest.Events
{
    public record BusEvent(PublishEntity Entity, string? LongString);
}
