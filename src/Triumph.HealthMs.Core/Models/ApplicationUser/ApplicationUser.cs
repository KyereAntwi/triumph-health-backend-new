namespace Triumph.HealthMs.Core.Models.ApplicationUser;

public class ApplicationUser : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public Nationality Nationality { get; set; } = Nationality.Ghanaian;
    public DateOnly DateOfBirth { get; set; }
    public string? OtherNames { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; }  = string.Empty;
    public string? ProfileImageUrl { get; set; }
}