using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Persistence;
using DistributedSystem.Persistence.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using System.Linq;

namespace DistributedSystem.Infrastructure.BackgroudJods;

/// <summary>
/// ngăn trường hợp duplicate Job
/// vì Excute đang chạy async và configure 100milisecond run
/// nên sễ có triongwf hợp jod cũ chưa chạy xong sẽ chạy jod mới
/// </summary>
[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint; //Maybe can use more Rebus library
    public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m=>m.ProcessedOnUtc == null)
            .OrderBy(m=>m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);
        foreach (var outboxMessage in messages)
        {
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject<IDomainEvent>(
                outboxMessage.Content,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                });

            if (domainEvent is null)
                continue;

            try
            {
                switch (domainEvent.GetType().Name)
                {
                    case nameof(DomainEvent.ProductCreated):
                        await PubLishEndpoint<DomainEvent.ProductCreated>(outboxMessage, context);
                        break;
                    case nameof(DomainEvent.ProductDeleted):
                        await PubLishEndpoint<DomainEvent.ProductDeleted>(outboxMessage, context);
                        break;
                    case nameof(DomainEvent.ProductUpdated):
                        await PubLishEndpoint<DomainEvent.ProductUpdated>(outboxMessage, context);
                        break;
                    default:
                        break;
                }
                //await _publishEndpoint.Publish(domainEvent, context.CancellationToken);
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;

            }
            catch (Exception ex)
            {
                outboxMessage.Error = ex.Message;
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task PubLishEndpoint<T>(OutboxMessage outboxMessage, IJobExecutionContext context)
    {
        T eventmessage = JsonConvert.DeserializeObject<T>(
                            outboxMessage.Content,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All
                            })!;

        await _publishEndpoint.Publish(eventmessage, context.CancellationToken);
    }
}
