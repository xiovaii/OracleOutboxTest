namespace OracleOutboxTest.Entities
{
    public class ConsumeEntity
    {
        public Guid Id { get; set; }
        public Guid CorelationId { get; set; }
        public string? Remark { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
