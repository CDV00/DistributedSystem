using DistributedSystem.Domain.Abstractions;
using DistributedSystem.Domain.Abstractions.Entities;
using DistributedSystem.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace DistributedSystem.Persistence.UnitOfWorks;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public EFUnitOfWork(ApplicationDbContext dbContext)
        => _dbContext = dbContext;

    public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //ConvertDomainEventsToOutboxMessages();
        //UpdateAuditableEntities();
        var a = await _dbContext.SaveChangesAsync();
        Console.WriteLine(a.ToString());
    }

    private void ConvertDomainEventsToOutboxMessages()
    {
        var outboxMessages = _dbContext.ChangeTracker
            .Entries<Domain.Abstractions.Aggregates.AggregateRoot<Guid>>()
            .Select(x=>x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent=> new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent, 
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    })
            }).ToList();
        _dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entities = _dbContext.ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entityEntry in entities)
        {
            if (entityEntry.State == EntityState.Added)
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            else if (entityEntry.State == EntityState.Modified)
                entityEntry.Property(a => a.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;

        }
    }
}
public class EFUnitOfWorkDbContext<TContext> : IUnitOfWorkDbContext<TContext>
    where TContext : DbContext
{
    private readonly TContext _dbContext;

    public EFUnitOfWorkDbContext(TContext dbContext)
        => _dbContext = dbContext;

    public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //ConvertDomainEventsToOutboxMessages();
        //UpdateAuditableEntities();
        await _dbContext.SaveChangesAsync();
    }

    private void ConvertDomainEventsToOutboxMessages()
    {
        var outboxMessages = _dbContext.ChangeTracker
            .Entries<Domain.Abstractions.Aggregates.AggregateRoot<Guid>>()
            .Select(x=>x.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent=> new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent, 
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    })
            }).ToList();
        _dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    }

    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<IAuditableEntity>> entities = _dbContext.ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entityEntry in entities)
        {
            if (entityEntry.State == EntityState.Added)
                entityEntry.Property(a => a.CreatedOnUtc).CurrentValue = DateTime.UtcNow;
            else if (entityEntry.State == EntityState.Modified)
                entityEntry.Property(a => a.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;

        }
    }
}
