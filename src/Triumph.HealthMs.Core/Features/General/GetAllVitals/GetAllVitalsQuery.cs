namespace Triumph.HealthMs.Core.Features.General.GetAllVitals;

public record GetAllVitalsQuery(
    string SearchKey = "", 
    int Page = 1, 
    int PageSize = 10);

public record VitalItemDto(string Id, string Name, string Description);
