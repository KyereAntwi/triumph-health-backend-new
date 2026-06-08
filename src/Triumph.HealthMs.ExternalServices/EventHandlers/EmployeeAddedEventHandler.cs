namespace Triumph.HealthMs.ExternalServices.EventHandlers;

public sealed class EmployeeAddedEventHandler (
    IServiceScopeFactory serviceScopeFactory, 
    ILogger<EmployeeAddedEventHandler> logger, 
    ISendMessage sendMessage,
    AppSettings appSettings)
    : IConsumer<EmployeeAddedEvent>
{
    public async Task Consume(ConsumeContext<EmployeeAddedEvent> context)
    {
        logger.LogInformation("Received EmployeeAddedEvent for employee with invitation id: {InvitationId}. Payload: {Event}", context.Message.EntityId, context.Message);
        
        var scope = serviceScopeFactory.CreateScope();
        var userDbContext = scope.ServiceProvider.GetRequiredService<IApplicationUserManagementDbContext>();

        var invitationLink = userDbContext
            .LinkInvitations
            .IgnoreQueryFilters()
            .Where(l => l.Id == context.Message.EntityId && !l.Deleted)
            .Select(l => new
            {
                l.ApplicationUserId,
                Fullname = l.ApplicationUser!.FirstName + " " + l.ApplicationUser.LastName,
                l.ApplicationUser.Email,
            })
            .FirstOrDefault();

        if (invitationLink is null)
        {
            logger.LogError("No invitation link found for EmployeeAddedEvent with invitation id: {InvitationId}", context.Message.EntityId);
            return;
        }
        
        var employeeDbContext = scope.ServiceProvider.GetRequiredService<IEmployeeManagementDbContext>();
        var employeeDetails = await employeeDbContext.Employees
            .IgnoreQueryFilters()
            .Where(e => e.ApplicationUserId == invitationLink.ApplicationUserId && !e.Deleted)
            .Select(e => new
            {
                e.FacilityId,
                e.UniqueIdentifier,
                e.CreatedAt,
                Role = e.EmployeeRoles.Select(er => er.Role!.Title).First()
            })
            .FirstAsync();
        
        var facilityDbContext = scope.ServiceProvider.GetRequiredService<IFacilityManagementDbContext>();
        var facilityDetails = await facilityDbContext.OrganizationalFacilities
            .IgnoreQueryFilters()
            .Where(f => f.Id == employeeDetails.FacilityId && !f.Deleted)
            .Select(f => new
            {
                f.Name,
                f.Email,
                f.MainTelephone,
                f.UrlSuffix,
                f.LogoUrl
            })
            .FirstAsync();
        
        var subject = $"Invitation to join {facilityDetails.Name} on Triumph Health";
        var message = UserInvitationTemplate.GetMessage(
            facilityDetails.Name,
            $"{facilityDetails.UrlSuffix}/{appSettings.MainDomain}/invitation?token={employeeDetails.UniqueIdentifier}",
            facilityDetails.LogoUrl ?? string.Empty,
            invitationLink.Fullname,
            employeeDetails.Role,
            facilityDetails.Email,
            facilityDetails.MainTelephone
        );

        var response = await sendMessage.SendEmailAsync(
            [invitationLink.Email!],
            null,
            subject,
            message
        );
    }
}