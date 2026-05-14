namespace Triumph.HealthMs.Core.Models.Common;

public class VitalItem : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}