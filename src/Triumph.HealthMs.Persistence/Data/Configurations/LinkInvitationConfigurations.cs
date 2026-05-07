namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class LinkInvitationConfigurations : IEntityTypeConfiguration<LinkInvitation>
{
    public void Configure(EntityTypeBuilder<LinkInvitation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x  => x.ApplicationUserId);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        builder.Property(x => x.InvitedEntityType)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.HasOne(x => x.ApplicationUser)
            .WithMany()
            .HasForeignKey(x => x.ApplicationUserId);
    }
}