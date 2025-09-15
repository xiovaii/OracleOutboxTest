using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OracleOutboxTest;
using OracleOutboxTest.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddDbContext<AppDbContext>(s => s.UseOracle(builder.Configuration.GetConnectionString("Oracle"), s => s.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
        o.UseOracle();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);

        cfg.UseMessageRetry(r =>
        {
            r.Immediate(int.MaxValue);
            r.Handle<DbUpdateConcurrencyException>();
        });

        cfg.ConfigureNewtonsoftJsonSerializer(settings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        cfg.UseNewtonsoftJsonSerializer();
        cfg.UseNewtonsoftJsonDeserializer();
    });
    x.AddConsumer<BusConsumer>();
    x.AddConsumer<PublishEndpointConsumer>();
    x.AddConsumer<ConsumeContextConsumer>();
});


var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
