using OracleOutboxTest.Entities;

namespace OracleOutboxTest.Events
{
    public record PublishEndpointEvent(PublishEntity Entity, string? LongString);
}
