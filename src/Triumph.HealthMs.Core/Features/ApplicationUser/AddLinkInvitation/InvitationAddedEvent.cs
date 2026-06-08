namespace Triumph.HealthMs.Core.Features.ApplicationUser.AddLinkInvitation;

public record InvitationAddedEvent(string InvitedEntityType) : IntegrationEvent;