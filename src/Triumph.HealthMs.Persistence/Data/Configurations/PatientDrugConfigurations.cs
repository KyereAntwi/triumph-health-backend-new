namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class PatientDrugConfigurations : IEntityTypeConfiguration<PatientDrug>
{
    public void Configure(EntityTypeBuilder<PatientDrug> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId);

        builder.Property(x => x.DrugId)
            .IsRequired();

        builder.Property(x => x.ExtraNotes)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.TenantId, x.FacilityId, x.PatientId, x.Deleted });
        builder.HasIndex(x => new { x.DrugId, x.AssociatedVisit, x.AssociatedDiagnosis, x.ActivelyTaking });
    }
}