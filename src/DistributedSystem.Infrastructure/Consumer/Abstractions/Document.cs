using MongoDB.Bson;

namespace DistributedSystem.Infrastructure.Consumer.Abstractions;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public Guid DocumentId { get; set; }//Id of source message: ProductId, OrderId, customerId

    public DateTimeOffset CreatedOnUtc => Id.CreationTime;

    public DateTimeOffset? ModifiedOnUtc { get; set; }
}