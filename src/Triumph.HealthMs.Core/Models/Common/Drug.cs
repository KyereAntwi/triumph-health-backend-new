namespace Triumph.HealthMs.Core.Models.Common;

public class Drug : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string? Manufacturer { get; set; }
}