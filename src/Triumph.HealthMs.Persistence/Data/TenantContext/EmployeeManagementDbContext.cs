namespace Triumph.HealthMs.Persistence.Data.TenantContext;

public sealed class EmployeeManagementDbContext : DbContext, IEmployeeManagementDbContext
{
    private readonly ILoggedInUserService _loggedInUserService;

    public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> opt, ILoggedInUserService loggedInUserService) : base(opt)
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
                    entityEntry.Entity.TenantId = (entityEntry.Entity.TenantId != null && entityEntry.Entity.TenantId != Guid.Empty)
                        ? entityEntry.Entity.TenantId
                        : Guid.Parse(_loggedInUserService.TenantId!);
                    entityEntry.Entity.FacilityId = (entityEntry.Entity.FacilityId != null && entityEntry.Entity.FacilityId != Guid.Empty)
                        ? entityEntry.Entity.FacilityId 
                        : Guid.Parse(_loggedInUserService.FacilityId!);
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeManagementDbContext).Assembly);
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
            
            var method = typeof(EmployeeManagementDbContext)
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

            var method = typeof(EmployeeManagementDbContext)
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
            
            var method = typeof(EmployeeManagementDbContext)
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

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeActivity> EmployeeActivities => Set<EmployeeActivity>();
    public DbSet<EmployeePermission> EmployeePermissions => Set<EmployeePermission>();
    public DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
    public DbSet<EmploymentAttachment> EmploymentAttachments => Set<EmploymentAttachment>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
}