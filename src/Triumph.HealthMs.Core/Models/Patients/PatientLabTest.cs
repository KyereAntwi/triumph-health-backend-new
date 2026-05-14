namespace Triumph.HealthMs.Core.Models.Patients;

public class PatientLabTest : FacilityEntity
{
    public Guid LabTestId { get; set; }
    public LabTest? LabTest { get; set; }

    public Guid VisitationId { get; set; }
    public Visitation? Visitation { get; set; }

    public string? ExtraNotes { get; set; }
    public string MeasuredValue { get; set; } = string.Empty;
    public decimal AmountPaid { get; set; }
    
    public Guid SupervisedById { get; set; }
    public Guid RecommendedById { get; set; }
}