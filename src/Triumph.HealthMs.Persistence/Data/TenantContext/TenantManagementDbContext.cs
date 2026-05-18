namespace Triumph.HealthMs.Persistence.Data.TenantContext;

public sealed class TenantManagementDbContext : DbContext, ITenantManagementDbContext
{
    private readonly ILoggedInUserService _loggedInUserService;

    public TenantManagementDbContext(
        DbContextOptions<TenantManagementDbContext> options, ILoggedInUserService loggedInUserService) : base(options)
    {
        _loggedInUserService = loggedInUserService;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entityEntry in ChangeTracker.Entries<TenantEntity>())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.CreatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.CreatedBy;
                    entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                    entityEntry.Entity.TenantId = (entityEntry.Entity.TenantId != null && entityEntry.Entity.TenantId != Guid.Empty)
                        ? entityEntry.Entity.TenantId
                        : Guid.Parse(_loggedInUserService.TenantId!);
                    break;
                case EntityState.Modified:
                    entityEntry.Entity.UpdatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.UpdatedBy;
                    entityEntry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        foreach (var entityEntry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entityEntry.State == EntityState.Added && !string.IsNullOrEmpty(_loggedInUserService.TenantId))
            {
                entityEntry.Entity.TenantId = Guid.Parse(_loggedInUserService.TenantId);
            }
        }
        
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantManagementDbContext).Assembly);
        ApplyDeletedFilter(modelBuilder);
        ApplyTenantFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    private void ApplyDeletedFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;
            
            var method = typeof(TenantManagementDbContext)
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

            var method = typeof(TenantManagementDbContext)
                .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [builder]);
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
    
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<TenantManager> TenantManagers => Set<TenantManager>();
}