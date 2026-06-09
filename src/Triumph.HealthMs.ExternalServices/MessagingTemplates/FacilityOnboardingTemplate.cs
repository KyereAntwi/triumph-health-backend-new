namespace Triumph.HealthMs.ExternalServices.MessagingTemplates;

public static class FacilityOnboardingTemplate
{
    public static string GetMessage(string tenantName, string facilityName, string logoUrl, string siteUrl) =>
    $"""
     <div style='font-family: Arial, sans-serif; color: #333; padding: 20px;'>
        <div style='text-align: center; margin-bottom: 30px'>
            <img src='{logoUrl}' alt='Triumph Health Logo' style='width: 150px;' />
        </div>
        
        <h1 style='color: #007BFF;'>Welcome to Triumph Health, {facilityName}!</h1>
        <p>You have been added to the facilities of {tenantName}</p>
        <p>We're excited to have you on board and looking forward to supporting your healthcare needs.</p>
        <p>You can access your facility portal at <a href='{siteUrl}'>{siteUrl}</a></p>
        
        <p>If you have any questions or need assistance, please don't hesitate to reach out to our support team at <a href='mailto:support@triumphhealth.online'>support@triumphhealth.com</a>.</p>
     
        <p>Best regards,<br>Triumph Health Team</p>
     </div>
     """;
}