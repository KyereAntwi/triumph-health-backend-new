namespace Triumph.HealthMs.ExternalServices.EventHandlers;

public sealed class PatientAddedEventHandler(
    IServiceScopeFactory scopeFactory, 
    ILogger<PatientAddedEventHandler> logger, 
    ISendMessage sendMessage) 
    : IConsumer<PatientAddedEvent>
{
    public async Task Consume(ConsumeContext<PatientAddedEvent> context)
    {
        logger.LogInformation("Received PatientAddedEvent for PatientId: {PatientId}. Payload: {Event}", context.Message.EntityId, context.Message);
        
        var scope = scopeFactory.CreateScope();
        var facilityDbContext = scope.ServiceProvider.GetRequiredService<IFacilityManagementDbContext>();
        var patientDbContext = scope.ServiceProvider.GetRequiredService<IPatientManagementDbContext>();
        var appUserDbContext = scope.ServiceProvider.GetRequiredService<IApplicationUserManagementDbContext>();
        
        var patient = await patientDbContext.Patients
            .Where(p => p.Id == context.Message.EntityId)
            .Select(p => new
            {
                p.ApplicationUserId,
                p.FacilityId
            })
            .FirstOrDefaultAsync();

        if (patient is null)
        {
            logger.LogError("Patient not found for PatientAddedEvent: {PatientId}", context.Message.EntityId);
        }
        
        var user = await appUserDbContext.ApplicationUsers
            .Where(u => u.Id == patient!.ApplicationUserId)
            .Select(u => new
            {
                Fullname = $"{u.FirstName} {u.LastName}",
                u.PhoneNumber
            })
            .FirstAsync();
        
        var facility = await facilityDbContext.OrganizationalFacilities
            .Where(f => f.Id == patient!.FacilityId)
            .Select(f => new
            {
                f.Name,
                f.Email,
                f.MainTelephone,
            })
            .FirstAsync();

        var message = $"""

                                  Dear {user.Fullname},

                                  Thank you for registering with {facility.Name}. We are excited to have you as part of our healthcare community.
                                  To get started, if you provided your email with us, please check your inbox and click the invitation link to activate your account. This will help us provide you with personalized care and support.

                                  If you have any questions or need assistance, please don't hesitate to contact our support team at [{facility.Email}] or call us at [{facility.MainTelephone}].

                                  Best regards,
                      """;

        var (success, error) = await sendMessage.SendSmsAsync(
            [user.PhoneNumber],
            null,
            message
        );

        if (!success)
        {
            logger.LogError("Error occured sending sms to patient. Error = {Error}. Event body {Event}", error, context.Message);
        }
    }
}