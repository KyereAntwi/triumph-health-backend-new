namespace Triumph.HealthMs.Core.Features.FacilityManagement.AddFacilityAnnouncement;

public sealed class AddFacilityAnnouncementCommandHandler(
    IFacilityManagementDbContext dbContext) 
    : ICommandHandler<AddFacilityAnnouncementCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddFacilityAnnouncementCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddFacilityAnnouncementValidator();
        var validationResult = await validation.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<Guid>
            {
                IsSuccess = false,
                Status = 400,
                Message = "Validation Failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
        
        var announcement = new FacilityAnnouncement
        {
            Message = command.Message,
            Type = Enum.Parse<AnnouncementType>(command.AnnouncementType),
            ValidUntil = DateTime.Parse(command.ValidUntil)
        };
        
        await dbContext.FacilityAnnouncements.AddAsync(announcement, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new BaseResponse<Guid>
        {
            IsSuccess = true,
            Message = "Announcement added successfully",
            Status = 201,
            Data = announcement.Id
        };
    }
}