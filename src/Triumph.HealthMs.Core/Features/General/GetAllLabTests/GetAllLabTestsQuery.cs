namespace Triumph.HealthMs.Core.Features.General.GetAllLabTests;

public record GetAllLabTestsQuery(
    string SearchKey = "",
    int Page = 1,
    int PageSize = 10);
    
public record LabTestDto(string Id, string Name, string Description);