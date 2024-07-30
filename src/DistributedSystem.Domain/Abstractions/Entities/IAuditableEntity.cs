namespace DistributedSystem.Domain.Abstractions.Entities;

public interface IAuditableEntity {
    DateTimeOffset CreatedOnUtc { get; protected set; }
    DateTimeOffset? ModifiedOnUtc { get; protected set; }
}
