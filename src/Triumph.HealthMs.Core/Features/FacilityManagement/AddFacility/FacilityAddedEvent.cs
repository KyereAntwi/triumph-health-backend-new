namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddFacility;

public record FacilityAddedEvent(Guid FacilityId) : IntegrationEvent;