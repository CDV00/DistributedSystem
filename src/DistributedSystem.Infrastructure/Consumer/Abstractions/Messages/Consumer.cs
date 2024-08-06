using DistributedSystem.Infrastructure.Consumer.Abstractions.Repositories;
using DistributedSystem.Infrastructure.Consumer.Models;
using MassTransit;
using MediatR;

namespace DistributedSystem.Infrastructure.Consumer.Abstractions.Messages;

public abstract class Consumer<TMessage> : IConsumer<TMessage>
    where TMessage : class, Contract.Abstractions.Message.IDomainEvent
{
    private readonly ISender Sender;
    private readonly IMongoRepository<EventProjection> _eventRepository;

    public Consumer(ISender sender, IMongoRepository<EventProjection> eventRepository) 
    {
        Sender = sender;
        _eventRepository = eventRepository;
    }
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        //Find by EventId
        //=> If Existed => Ignore
        //=> Not Existed => Create EventProjection
        var eventProjection = await _eventRepository.FindOneAsync(e => e.EventId == context.Message.IdEvent);
        if(eventProjection is null)
        {
            eventProjection = new EventProjection()
            {
                EventId = context.Message.IdEvent,
                Name = context.Message.GetType().Name,
                Type = context.Message.GetType().Name,
            };
            await _eventRepository.InsertOneAsync(eventProjection);

            await Sender.Send(context.Message); //dont call __dbContext.SaveChangeAsync() in it
        }
    }
}
