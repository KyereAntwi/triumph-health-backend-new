namespace Triumph.HealthMs.Host.Services;

public class LoggedInUserService(IHttpContextAccessor httpContextAccessor) : ILoggedInUserService
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext ?? throw new UnauthorizedAccessException("User is missing");

    public string? UserId => HttpContext.User.FindFirst("sub")?.Value;

    public string? TenantId
    {
        get
        {
            return HttpContext.Request.Headers["x-ms-tenant-id"].FirstOrDefault() ??
                   HttpContext.User.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value ??
                   null;
        }
    }

    public string? FacilityId
    {
        get
        {
            return HttpContext.Request.Headers["x-ms-facility-id"].FirstOrDefault() ??
                   HttpContext.User.Claims.FirstOrDefault(c => c.Type == "facility_id")?.Value ??
                   null;
        }
    }
}