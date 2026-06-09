namespace Triumph.HealthMs.ExternalServices.MessagingTemplates;

public static class TenantOnboardingTemplate
{
    public static string GetMessage(string tenantName, string logoUrl, string subscription, DateTime expiryDate, SubscriptionChargeRate rate ) => 
    $"""
        <div style='font-family: Arial, sans-serif; color: #333; padding: 20px;'>
            
            <div style='text-align: center; margin-bottom: 30px;'>
                <img src='{logoUrl}' alt='Triumph Health Logo' style='width: 150px;' />
            </div>
            
            <h1 style='color: #007BFF;'>Welcome to Triumph Health, {tenantName}!</h1>
            <p>Thank you for creating an account with us. We're excited to have you on board and look forward to supporting your healthcare needs.</p>
            <h2 style='color: #007BFF;'>Your Subscription Details:</h2>
            
            <ul>
                <li><strong>Subscription Plan:</strong> {subscription}</li>
                <li><strong>Expires At:</strong> {expiryDate:MMMM dd, yyyy}</li>
                <li><strong>Charge Rate:</strong> {rate.ToString()}</li>
            </ul>
            
            <p>If you have any questions or need assistance, please don't hesitate to reach out to our support team at <a href='mailto:support@triumphhealth.com'>support@triumphhealth.online</a>.</p>
            
            <p>Best regards,<br>Triumph Health Team</p>
        </div>
     """;
}