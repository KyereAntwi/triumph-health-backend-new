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
            from = from ?? resendSettings.FromEmail,
            to = tos,
            subject = subject,
            html = htmlContent
        };

        if (attachments != null)
        {
            // todo - process attachments
        }

        try
        {
            var response = await resendClient.PostAsJsonAsync("/emails", body);
            response.EnsureSuccessStatusCode();
            return (true, null);
        }
        catch (Exception e)
        {
            logger.LogError(e, "There was a problem sending the email. Email body: {Body} ", body);
            return (false, e.Message);
        }
    }

    public async Task<(bool, string?)> SendSmsAsync(List<string> tos, string? from, string message)
    {
        var arkesselClient = httpClientFactory.CreateClient("arkessel");
        var body = new
        {
            sender = from ?? arkesselSettings.Sender,
            message = message,
            recipients = tos
        };

        try
        {
            var response = await arkesselClient.PostAsJsonAsync("/sms/send", body);
            response.EnsureSuccessStatusCode();
            return (true, null);
        }
        catch (Exception e)
        {
            logger.LogError(e, "There was a problem sending the SMS. SMS body: {Body} ", body);
            return (false, e.Message);
        }
    }
}