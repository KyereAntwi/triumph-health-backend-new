namespace Triumph.HealthMs.Core.Models.Common;

public class AuditableEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool Deleted { get; set; } = false;
}