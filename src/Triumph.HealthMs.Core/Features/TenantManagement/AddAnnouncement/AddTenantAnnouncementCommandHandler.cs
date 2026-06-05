namespace Triumph.HealthMs.Core.Features.TenantManagement.AddAnnouncement;

public sealed class AddTenantAnnouncementCommandHandler(
    ITenantManagementDbContext dbContext,
    ILoggedInUserService loggedInUserService) 
    : ICommandHandler<AddTenantAnnouncementCommand, Guid>
{
    public async Task<BaseResponse<Guid>> HandleAsync(AddTenantAnnouncementCommand command, CancellationToken cancellationToken = default)
    {
        var validation = new AddTenantAnnouncementCommandValidator();
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

        var announcement = new TenantAnnouncement
        {
            Message = command.Message,
            Type = Enum.Parse<AnnouncementType>(command.AnnouncementType),
            ValidUntil = DateTime.Parse(command.ValidUntil)
        };
        
        await dbContext.TenantAnnouncements.AddAsync(announcement, cancellationToken);
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