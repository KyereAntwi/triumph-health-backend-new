namespace Triumph.HealthMs.Core.Features.TenantManagement.UpdateAnnouncement;

public sealed class UpdateTenantAnnouncementCommandHandler(
    ITenantManagementDbContext dbContext) 
    : ICommandHandler<UpdateTenantAnnouncementCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateTenantAnnouncementCommand command, CancellationToken cancellationToken = default)
    {
        var validator = new UpdateTenantAnnouncementCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        var announcement = await dbContext.TenantAnnouncements
            .Where(a => a.Id == command.AnnouncementId)
            .FirstOrDefaultAsync(cancellationToken);

        if (announcement is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Announcement was not found"]
            };
        }
        
        announcement.Message = command.Message;
        announcement.ValidUntil = DateTime.Parse(command.ValidUntil);
        announcement.Type = Enum.Parse<AnnouncementType>(command.AnnouncementType);
        
        dbContext.TenantAnnouncements.Update(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Announcement updated successfully"
        };
    }
}