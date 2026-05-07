namespace Triumph.HealthMs.Persistence.Data;

public class TenantManagementDbContext : DbContext, ITenantManagementDbContext
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
        foreach (var entityEntry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Entity.CreatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.CreatedBy;
                    entityEntry.Entity.CreatedAt = DateTime.UtcNow;
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantManagementDbContext).Assembly);
        ApplyDeletedFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<LinkInvitation> LinkInvitations => Set<LinkInvitation>();

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
    private void SetDeletedFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : AuditableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.Deleted);
    }
}