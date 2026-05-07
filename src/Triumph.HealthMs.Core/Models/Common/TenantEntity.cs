namespace Triumph.HealthMs.Core.Models.Common;

public class TenantEntity : AuditableEntity
{
    public Guid? TenantId { get; set; }
}