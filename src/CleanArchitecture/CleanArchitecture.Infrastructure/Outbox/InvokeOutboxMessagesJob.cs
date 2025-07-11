using System.Data;
using System.Drawing.Text;
using CleanArchitecture.Application.Abstractions.Clock;
using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace CleanArchitecture.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class InvokeOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly OutboxOptions _outboxOptions;
    private readonly ILogger<InvokeOutboxMessagesJob> _logger;

    public InvokeOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<InvokeOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Invoking outbox messages");

        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        var sql = $@"
        SELECT 
            id, content 
            FROM outbox_messages
        WHERE processed_on_utc IS NULL
        ORDER BY ocurred_on_utc
        LIMIT {_outboxOptions.BatchSize}
        FOR UPDATE
        ";

        var records = (await connection.QueryAsync<OutBoxMessageData>(sql, transaction)).ToList();

        foreach (var message in records)
        {
            Exception? exception = null;
            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    message.Content,
                    _jsonSerializerSettings
                    )!;

                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing outbox message {MessageId}", message.Id);

                exception = ex;
            }

            await UpdateOutboxMessage(connection, transaction, message, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Invoked {Count} outbox messages successfully", records.Count);
    }

    private async Task UpdateOutboxMessage(
        IDbConnection connection,
        IDbTransaction transaction,
        OutBoxMessageData message,
        Exception? exception)
    {
        var sql = """
                    UPDATE outbox_messages
                    SET processed_on_utc = @ProcessedOnUtc, error = @Error
                    WHERE id = @Id
                    """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                message.Id,
                ProcessedOnUtc = _dateTimeProvider.CurrenTime,
                Error = exception?.ToString()
            },
            transaction);
    }
}


public record OutBoxMessageData(Guid Id, string Content);