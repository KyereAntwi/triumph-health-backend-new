namespace Triumph.HealthMs.Host.Services;

public class LoggedInUserService(IHttpContextAccessor httpContextAccessor) : ILoggedInUserService
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext 
                                       ?? throw new UnauthorizedAccessException("User is missing");

    public string? UserId => HttpContext.User.FindFirst("sub")?.Value;

    public string? TenantId => HttpContext.Request.Headers["x-ms-tenant-id"].FirstOrDefault() ?? null;

    public string? FacilityId => HttpContext.Request.Headers["x-ms-facility-id"].FirstOrDefault() ?? null;

    public string? FacilityUrlPrefix
    {
        get
        {
            var host = HttpContext.Request.Host.Host;
            
            if(host.Contains("api"))
                return null;
            
            if (host.Contains("localhost"))
            {
                var localhostParts = host.Split('.');
                return localhostParts.Length > 1
                    ? localhostParts[0]
                    : null;
            }
            
            var parts = host.Split('.');
            return parts.Length < 3 ? null : parts[0];
        }
    }
}