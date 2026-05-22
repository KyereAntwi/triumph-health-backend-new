namespace Triumph.HealthMs.Core.Features.General.GetAllDrugs;

public record GetAllDrugsQuery(
    string SearchKey = "",
    int Page = 1,
    int PageSize = 10);
    
public record DrugDto(
    string Id,
    string Name,
    string Description,
    string Prescription,
    string Manufacturer);