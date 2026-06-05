namespace Triumph.HealthMs.Core.Features.FacilityManagement.UpdateFacilityAnnouncement;

public record UpdateFacilityAnnouncementCommand(
    string Message,
    string AnnouncementType,
    string ValidUntil)
{
    public Guid Id { get; set; }
    public Guid AnnouncementId { get; set; }
}

public class UpdateFacilityAnnouncementCommandValidator : AbstractValidator<UpdateFacilityAnnouncementCommand>
{
    public UpdateFacilityAnnouncementCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MaximumLength(1000)
            .WithMessage("Message must be less than 1000 characters.");

        RuleFor(x => x.ValidUntil)
            .NotEmpty()
            .WithMessage("Valid until date is required.")
            .Must(x => DateTime.TryParse(x, out _))
            .WithMessage("Invalid date format for valid until.")
            .Must(x => DateTime.TryParse(x, out var date) && date > DateTime.UtcNow)
            .WithMessage("Valid until date must be in the future.");

        RuleFor(x => x.AnnouncementType)
            .NotEmpty()
            .WithMessage("Announcement type is required.")
            .Must(x => Enum.TryParse<AnnouncementType>(x, out _))
            .WithMessage("Invalid announcement type.");
    }
}