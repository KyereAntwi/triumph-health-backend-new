namespace Triumph.HealthMs.Persistence.Data.Configurations;

public sealed class EmployeeShiftConfigurations : IEntityTypeConfiguration<EmployeeShift>
{
    public void Configure(EntityTypeBuilder<EmployeeShift> builder)
    {
        builder.ToTable("EmployeeShifts");
        builder.HasKey(es => es.Id);
        
        builder.HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeShifts)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(es => es.ShiftType)
            .IsRequired();

        builder.HasIndex(es => new { es.TenantId, es.FacilityId, es.EmployeeId, es.Deleted });
        builder.HasIndex(es => es.ShiftType);
        builder.HasIndex(es => es.DayOfWeek);
        builder.HasIndex(es => es.CreatedAt);
    }
}