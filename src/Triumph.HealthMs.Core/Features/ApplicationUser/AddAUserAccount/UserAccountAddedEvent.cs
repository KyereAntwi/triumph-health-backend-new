namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddAUserAccount;

public record UserAccountAddedEvent : IntegrationEvent
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}