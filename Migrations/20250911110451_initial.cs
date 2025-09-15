using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OracleOutboxTest.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CorelationId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Remark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MessageId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    LockId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "RAW(8)", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ReceiveCount = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Consumed = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Delivered = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    LockId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "RAW(8)", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Delivered = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Publishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Remark = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EnqueueTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    SentTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Headers = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Properties = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    OutboxId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    MessageId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ContentType = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Body = table.Column<string>(type: "CLOB", nullable: false),
                    ConversationId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    RequestId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    SourceAddress = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "\"InboxMessageId\" IS NOT NULL AND \"InboxConsumerId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "\"OutboxId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consumes");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "Publishes");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");
        }
    }
}
