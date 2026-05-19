namespace Triumph.HealthMs.Persistence.Interceptors;

public sealed class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly ILogger<AuditingInterceptor> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuditingInterceptor(ILoggedInUserService loggedInUserService, ILogger<AuditingInterceptor> logger, IPublishEndpoint publishEndpoint)
    {
        _loggedInUserService = loggedInUserService;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;
        if (context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);
        
        var entries = context.ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Modified or EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.Entity.Deleted = true;
                entry.Entity.DeletedAt = DateTimeOffset.UtcNow;
                entry.Entity.DeletedBy = _loggedInUserService.UserId ?? entry.Entity.DeletedBy;
                entry.State = EntityState.Modified; // Mark as modified to update the entity instead of deleting it
            }
            else
            {
                var @event = new SaveAuditEvent
                {
                    UserId = _loggedInUserService.UserId!,
                    Action = entry.State.ToString(),
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id,
                    Before = entry.OriginalValues.ToObject(),
                    After = entry.CurrentValues.ToObject(),
                    TraceId = Activity.Current?.TraceId.ToString()
                };

                try
                {
                    await _publishEndpoint.Publish(@event, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to publish audit event for entity {EntityType} with state {EntityState}. TraceId: {TraceId} and PayloadId: {PayloadId}",
                        entry.Entity.GetType().Name, 
                        entry.State, 
                        Activity.Current?.TraceId, 
                        @event);
                }
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
        CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            _logger.LogError(
                eventData.Exception,
                "Failed to save changes for entity {EntityType} with state {EntityState}. TraceId: {TraceId} and PayloadId: {PayloadId}",
                entry.Entity.GetType().Name, 
                entry.State, 
                Activity.Current?.TraceId, 
                entry.OriginalValues.ToObject());
        }
        
        await base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}