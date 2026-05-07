namespace Triumph.HealthMs.Core.Models.Tenants;

public class Subscription : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float CostPerMonth { get; set; } = 0;
}