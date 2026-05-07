namespace Triumph.HealthMs.Core.Models;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid? TenantId { get; set; }
    public Guid? FacilityId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public object? Before { get; set; }
    public object? After { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }
}