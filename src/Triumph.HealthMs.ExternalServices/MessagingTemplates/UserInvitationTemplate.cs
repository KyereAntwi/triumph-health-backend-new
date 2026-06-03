namespace Triumph.HealthMs.ExternalServices.MessagingTemplates;

public static class UserInvitationTemplate
{
    public static string GetMessage(string facilityName, string invitationLink, string facilityLogoUrl, string user, string role, string facilityEmail, string facilityPhone) =>
    $"""
        <div style='font-family: Arial, sans-serif; color: #333; padding: 20px;'>
            
            <div style='text-align: center; margin-bottom: 30px;'>
                <img src='{facilityLogoUrl}' alt='{facilityName} Logo' style='width: 150px;' />
            </div>
            
            <h1 style='color: #007BFF;'>You're Invited to Join {facilityName} on Triumph Health!</h1>
            <p>Hello {user},</p>
            <p>You have been invited to join {facilityName} as a {role} on the Triumph Health platform. To accept this invitation and set up your account, please click the link below:</p>
            
            <p><a href='{invitationLink}' style='display: inline-block; padding: 10px 20px; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px;'>Accept Invitation</a></p>
            
            <p>If you have any questions or need assistance, please contact {facilityName} at:</p>
            <ul>
                <li><strong>Email:</strong> {facilityEmail}</li>
                <li><strong>Phone:</strong> {facilityPhone}</li>
            </ul>
            
            <p>Best regards,<br>{facilityName} Team</p>
            
            <em style="font-size: 0.8em; color: gray;">This email is sent by Triumph Health Team, please do not reply.</em>
        </div>
     """;
}