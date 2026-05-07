namespace Triumph.HealthMs.Core.Events;

public record IntegrationEvent
{
    public string EventType => GetType().AssemblyQualifiedName!;
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
}