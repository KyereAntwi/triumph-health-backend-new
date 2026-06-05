namespace Triumph.HealthMs.Core.Features.AppConfigurations.GetAppConfigs;

public record ConfigsResponse
{
    public UserInformationDto? UserInformation { get; set; } = null;
    public TenantInformationDto? TenantInformation { get; set; } = null;
    public FacilityInformationDto? FacilityInformation { get; set; } = null;
    public RoleDto? RoleInformation { get; set; } = null;
    public IEnumerable<string> Permissions { get; set; } = [];
    public IEnumerable<AnnouncementDto> Announcements { get; set; } = [];
}

public record UserInformationDto(
    string Title,
    string FirstName,
    string? OtherNames,
    string LastName,
    string ProfileImageUrl,
    string Email);
    
public record TenantInformationDto(
    string Id, 
    string OrganizationTitle, 
    string LogoUrl, 
    string Address,
    bool HasValidSubscription);

public record FacilityInformationDto(string Id, string Name, string LogoUrl, string Address);

public record RoleDto(
    string EntityType,
    string? Role,
    bool IsTenantManager,
    bool IsFacilityManager);
    
public record AnnouncementDto(
    string Id,
    string Message,
    string Type,
    string Entity);