namespace Triumph.HealthMs.Persistence.Data.Configurations;

public class OrganizationalFacilityConfigurations : IEntityTypeConfiguration<OrganizationalFacility>
{
    public void Configure(EntityTypeBuilder<OrganizationalFacility> builder)
    {
        builder.ToTable("OrganizationalFacilities");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.UrlSuffix)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.MainTelephone)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(255);

        builder.HasMany(x => x.FacilityManagers)
            .WithOne(x => x.OrganizationalFacility)
            .HasForeignKey(x => x.FacilityId);
    }
}