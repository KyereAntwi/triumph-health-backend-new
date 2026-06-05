namespace Triumph.HealthMs.Core.Features.FacilityManagement.UpdateFacilityAnnouncement;

public sealed class UpdateFacilityAnnouncementCommandHandler(IFacilityManagementDbContext dbContext) 
    : ICommandHandler<UpdateFacilityAnnouncementCommand, string>
{
    public async Task<BaseResponse<string>> HandleAsync(UpdateFacilityAnnouncementCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new UpdateFacilityAnnouncementCommandValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);
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
        
        var announcement = await dbContext
            .FacilityAnnouncements
            .AsTracking()
            .Where(a => a.Id == command.AnnouncementId)
            .FirstOrDefaultAsync(cancellationToken);

        if (announcement is null)
        {
            return new BaseResponse<string>
            {
                IsSuccess = false,
                Status = 404,
                Message = "Not Found",
                Errors = ["Announcement not found"]
            };
        }
        
        announcement.Message = command.Message;
        announcement.Type = Enum.Parse<AnnouncementType>(command.AnnouncementType);
        announcement.ValidUntil = DateTime.Parse(command.ValidUntil);
        
        dbContext.FacilityAnnouncements.Update(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new BaseResponse<string>
        {
            IsSuccess = true,
            Status = 200,
            Message = "Announcement updated successfully",
        };
    }
}