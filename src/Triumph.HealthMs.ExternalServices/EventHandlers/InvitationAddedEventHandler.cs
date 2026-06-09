namespace Triumph.HealthMs.ExternalServices.EventHandlers;

public sealed class InvitationAddedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<InvitationAddedEventHandler> logger,
    ISendMessage sendMessage,
    AppSettings appSettings) 
    : IConsumer<InvitationAddedEvent>
{
    public async Task Consume(ConsumeContext<InvitationAddedEvent> context)
    {
        logger.LogInformation("Received InvitationAddedEvent. Payload: {Payload}", context.Message);

        var scope = serviceScopeFactory.CreateScope();
        var userDbContext = scope.ServiceProvider.GetRequiredService<IApplicationUserManagementDbContext>();
        
        var invitationLink = await userDbContext
            .LinkInvitations
            .IgnoreQueryFilters()
            .Where(l => l.Id == context.Message.EntityId && !l.Deleted)
            .Select(l => new
            {
                l.ApplicationUserId,
                Fullname = l.ApplicationUser!.FirstName + " " + l.ApplicationUser.LastName,
                l.ApplicationUser.Email,
            })
            .FirstOrDefaultAsync();
        
        if (invitationLink is null)
        {
            logger.LogError("No invitation link found for InvitationAddedEventHandler with invitation id: {InvitationId}", context.Message.EntityId);
            return;
        }

        var role = string.Empty;
        var facilityId = Guid.Empty;

        if (string.Equals(context.Message.InvitedEntityType, "Employee", StringComparison.OrdinalIgnoreCase))
        {
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
            
            role = employeeDetails.Role;
            facilityId = (Guid)employeeDetails.FacilityId!;
        }

        if (string.Equals(context.Message.InvitedEntityType, "Patient", StringComparison.OrdinalIgnoreCase))
        {
            //todo - implement for patient scenario
        }
        
        var facilityDbContext = scope.ServiceProvider.GetRequiredService<IFacilityManagementDbContext>();
        var facilityDetails = await facilityDbContext.OrganizationalFacilities
            .IgnoreQueryFilters()
            .Where(f => f.Id == facilityId && !f.Deleted)
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
            $"{facilityDetails.UrlSuffix}.facilities-app.{appSettings.MainDomain}/invitation?token={context.Message.EntityId}",
            facilityDetails.LogoUrl ?? string.Empty,
            invitationLink.Fullname,
            role,
            facilityDetails.Email,
            facilityDetails.MainTelephone
        );
        
        await sendMessage.SendEmailAsync(
            [invitationLink.Email!],
            null,
            subject,
            message
        );
    }
}