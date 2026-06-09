namespace Triumph.HealthMs.ExternalServices.Services;

public sealed class MessagingServices(
    IHttpClientFactory httpClientFactory, 
    ILogger<MessagingServices> logger,
    ResendSettings resendSettings,
    ArkesselSettings arkesselSettings) 
    : ISendMessage
{
    public async Task<(bool, string?)> SendEmailAsync(List<string> tos, string? from, string subject, string htmlContent, List<string>? attachments = null)
    {
        var resendClient = httpClientFactory.CreateClient("resend");
        var body = new
        {
            from = resendSettings.FromEmail,
            to = tos,
            subject,
            html = htmlContent,
            attachments
        };

        var response = await resendClient.PostAsJsonAsync("/emails", body);

        if (response.IsSuccessStatusCode) return (true, null);
        
        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("There was a problem sending the email. Error: {Error} Email body: {Body} ", error, body);
        return (false, error);
    }

    public async Task<(bool, string?)> SendSmsAsync(List<string> tos, string? from, string message)
    {
        var arkesselClient = httpClientFactory.CreateClient("arkessel");
        var body = new
        {
            sender = from ?? arkesselSettings.Sender,
            message,
            recipients = tos
        };
        
        var response = await arkesselClient.PostAsJsonAsync("/sms/send", body);

        if (response.IsSuccessStatusCode) return (true, null);
        
        var error = await response.Content.ReadAsStringAsync();
        logger.LogError("There was a problem sending the SMS. Error: {Error} SMS body: {Body} ", error, body);
        return (false, error);
    }
}