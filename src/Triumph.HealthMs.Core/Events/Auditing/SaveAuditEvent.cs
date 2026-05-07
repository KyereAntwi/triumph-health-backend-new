namespace Triumph.HealthMs.Core.Events.Auditing;

public record SaveAuditEvent : IntegrationEvent
{
    public object Before { get; set; } = null!;
    public object After { get; set; } = null!;
    public string? TraceId { get; set; }
}