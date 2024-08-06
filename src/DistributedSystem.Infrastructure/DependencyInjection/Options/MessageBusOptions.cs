using System.ComponentModel.DataAnnotations;

namespace DistributedSystem.Infrastructure.DependencyInjection.Options;

public class MessageBusOptions
{
    [Required, Range(1, 10)] public int RetryLimit { get; set; }
    [Required, Timestamp] public TimeSpan InitialInterval { get; set; }
    [Required, Timestamp] public TimeSpan InitialIncrement { get; set; }
}