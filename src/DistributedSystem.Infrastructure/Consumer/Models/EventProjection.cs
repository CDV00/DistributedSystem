using DistributedSystem.Infrastructure.Consumer.Abstractions;
using DistributedSystem.Infrastructure.Consumer.Attributes;
using DistributedSystem.Infrastructure.Consumer.Constants;

namespace DistributedSystem.Infrastructure.Consumer.Models;

[BsonCollection(TableNames.Event)]
public class EventProjection : Document
{
    public Guid EventId { get; set; }
    public string Name { set; get; }
    public string Type { set; get; }
}
