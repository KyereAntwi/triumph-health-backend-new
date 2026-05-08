namespace Triumph.HealthMs.Core.Features.FacilityManagement.RemoveManager;

public record RemoveFacilityManagerCommand
{
    public Guid FacilityId { get; set; }
    public Guid ManagerId { get; set; }
};