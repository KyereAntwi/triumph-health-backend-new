namespace Triumph.HealthMs.Core.Features.General.GetAllSubscriptions;

public record GetAllSubscriptionsQuery;

public record SubscriptionDto(string Id, string Title, string Description, float CostPerMonth);