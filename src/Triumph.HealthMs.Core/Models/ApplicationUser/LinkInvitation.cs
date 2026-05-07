namespace Triumph.HealthMs.Core.Models.ApplicationUser;

public class LinkInvitation : TenantEntity
{
    public DateTime ExpiresAt { get; set; } =  DateTime.UtcNow.AddDays(7);
    public string InvitedEntityType { get; set; } = string.Empty;
    public bool Linked { get; set; } = false;

    public Guid ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}