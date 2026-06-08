namespace Triumph.HealthMs.Persistence.Data.CommonEntitiesContext;

public sealed class ApplicationUserManagementDbContext : DbContext, IApplicationUserManagementDbContext
{
    private readonly ILoggedInUserService _loggedInUserService;

    public ApplicationUserManagementDbContext(
        DbContextOptions<ApplicationUserManagementDbContext> options, ILoggedInUserService loggedInUserService) : base(options)
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
                    entityEntry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entityEntry.Entity.UpdatedBy = _loggedInUserService.UserId ?? entityEntry.Entity.UpdatedBy;
                    entityEntry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                    break;
            }
        }
        
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserManagementDbContext).Assembly);
        ApplyDeletedFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<LinkInvitation> LinkInvitations => Set<LinkInvitation>();
    public DbSet<UiStorageItem> UiStorageItems => Set<UiStorageItem>();

    private void ApplyDeletedFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType)) continue;
            
            var method = typeof(ApplicationUserManagementDbContext)
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