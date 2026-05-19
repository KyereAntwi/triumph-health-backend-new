namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class IdentificationConfigurations : IEntityTypeConfiguration<Identification>
{
    public void Configure(EntityTypeBuilder<Identification> builder)
    {
        builder.ToTable("Identifications");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.PatientId, x.Deleted, x.Type });

        builder.Property(x => x.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(15);
        builder.Property(x => x.PlaceOfIssue)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.CountryOfIssue)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DateExpires)
            .IsRequired();
        builder.Property(x => x.DateIssued)
            .IsRequired();

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Identifications)
            .HasForeignKey(x => x.PatientId);
    }
}