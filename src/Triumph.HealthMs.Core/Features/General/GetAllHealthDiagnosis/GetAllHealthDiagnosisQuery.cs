namespace Triumph.HealthMs.Core.Features.General.GetAllHealthDiagnosis;

public record GetAllHealthDiagnosisQuery(
    string SearchKey = "",
    int Page = 1,
    int PageSize = 10);
    
public record HealthDiagnosisDto(string Id, string Name, string Description, string RecommendedPrescription);