namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .HasMaxLength(5);
        
        builder.HasIndex(x => x.UserId)
            .IsUnique();
        
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => new { x.FirstName, x.LastName });

        builder.Property(x => x.OtherNames)
            .HasMaxLength(50);
        
        builder.Property(x => x.Gender)
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255);
        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(15)
            .IsRequired();

        builder.HasIndex(x => new { x.Email, x.PhoneNumber });

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(255);

        builder.HasIndex(x => x.Deleted);
    }
}