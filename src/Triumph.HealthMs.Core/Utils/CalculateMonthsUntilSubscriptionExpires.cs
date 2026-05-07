namespace Triumph.HealthMs.Core.Utils;

public static class CalculateMonthsUntilSubscriptionExpires
{
    public static int Calculate(SubscriptionChargeRate rate)
    {
        var additionalMonthsUntilExpires = rate switch
        {
            SubscriptionChargeRate.Monthly => 1,
            SubscriptionChargeRate.HalfYearly => 6,
            SubscriptionChargeRate.Quarterly => 4,
            SubscriptionChargeRate.Yearly => 12,
            _ => throw new InvalidDataException("Invalid subscription charge rate")
        };

        return additionalMonthsUntilExpires;
    }
}