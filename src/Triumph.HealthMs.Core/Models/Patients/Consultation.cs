namespace Triumph.HealthMs.Core.Models.Patients;

public class Consultation : FacilityEntity
{
    public Guid VisitationId { get; set; }
    public Visitation? Visitation { get; set; }

    public string Notes { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    public string? Room { get; set; }
}