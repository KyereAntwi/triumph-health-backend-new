namespace Triumph.HealthMs.Persistence.Data.SeedData;

public class SeedSubscriptions : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasData([
            new Subscription
            {
                Id = Guid.Parse("133e4567-e89b-12d3-a456-426655440000"),
                CreatedBy = "System",
                CreatedAt = DateTimeOffset.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedAt = DateTimeOffset.Parse("Sat, 09 May 2026 20:22:30 GMT"),
                UpdatedBy = "System",
                Deleted = false,
                Title = "Free",
                Description = "Free subscription plan with all basic support.",
                CostPerMonth = 00.00f
            }
        ]);
    }
}