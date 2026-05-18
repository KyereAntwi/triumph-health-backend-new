namespace Triumph.HealthMs.Persistence.Data.TenantContext;

public sealed class PatientManagementDbContext : DbContext, IPatientManagementDbContext
{
    private readonly ILoggedInUserService _loggedInUserService;

    public PatientManagementDbContext(DbContextOptions<PatientManagementDbContext> opt, ILoggedInUserService loggedInUserService) : base(opt)
    {
        _loggedInUserService = loggedInUserService;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entityEntry in ChangeTracker.Entries<FacilityEntity>())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.CreatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.CreatedBy;
                    entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                    entityEntry.Entity.TenantId = Guid.Parse(_loggedInUserService.TenantId!);
                    entityEntry.Entity.FacilityId = Guid.Parse(_loggedInUserService.FacilityId!);
                    break;
                case EntityState.Modified:
                    entityEntry.Entity.UpdatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.UpdatedBy;
                    entityEntry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientManagementDbContext).Assembly);
        ApplyDeletedFilter(modelBuilder);
        ApplyTenantFilter(modelBuilder);
        ApplyFacilityFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    private void ApplyDeletedFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;
            
            var method = typeof(PatientManagementDbContext)
                .GetMethod(nameof(SetDeletedFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);
            
            method.Invoke(this, [modelBuilder]);
        }
    }
    
    private void ApplyTenantFilter(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(TenantEntity).IsAssignableFrom(entityType.ClrType)) continue;

            var method = typeof(PatientManagementDbContext)
                .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [builder]);
        }
    }
    
    private void ApplyFacilityFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(!typeof(FacilityEntity).IsAssignableFrom(entityType.ClrType)) continue;
            
            var method = typeof(PatientManagementDbContext)
                .GetMethod(nameof(SetFacilityFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);
            
            method.Invoke(this, [modelBuilder]);
        }
    }
    
    private void SetDeletedFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : AuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.Deleted);
    }
    private void SetTenantFilter<TEntity>(ModelBuilder builder) where TEntity : TenantEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == Guid.Parse(_loggedInUserService.TenantId!));
    }
    private void SetFacilityFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : FacilityEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.FacilityId == Guid.Parse(_loggedInUserService.FacilityId!));
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Identification> Identifications => Set<Identification>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<PatientDrug> PatientDrugs => Set<PatientDrug>();
    public DbSet<PatientHealthDiagnosis> PatientHealthDiagnoses => Set<PatientHealthDiagnosis>();
    public DbSet<PatientLabTest> PatientLabTests => Set<PatientLabTest>();
    public DbSet<PatientVital> PatientVitals => Set<PatientVital>();
    public DbSet<Visitation> Visitations => Set<Visitation>();
    public DbSet<HealthDiagnosis> HealthDiagnoses => Set<HealthDiagnosis>();
    public DbSet<VitalItem> VitalItems => Set<VitalItem>();
}