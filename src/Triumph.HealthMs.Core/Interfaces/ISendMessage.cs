namespace Triumph.HealthMs.Core.Interfaces;

public interface ISendMessage
{
    Task<(bool, string?)> SendEmailAsync(List<string> tos, string? from, string subject, string htmlContent, List<string>? attachments = null);
    Task<(bool, string?)> SendSmsAsync(List<string> tos, string? from, string message);
}