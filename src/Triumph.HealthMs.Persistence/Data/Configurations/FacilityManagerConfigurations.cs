namespace Triumph.HealthMs.Persistence.Data.Configurations;

public class FacilityManagerConfigurations : IEntityTypeConfiguration<FacilityManager>
{
    public void Configure(EntityTypeBuilder<FacilityManager> builder)
    {
        builder.ToTable("FacilityManagers");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TenantId, x.FacilityId });
        builder.HasIndex(x => x.ApplicationUserId);

        builder.HasOne(x => x.OrganizationalFacility)
            .WithMany(x => x.FacilityManagers)
            .HasForeignKey(x => x.FacilityId);
    }
}