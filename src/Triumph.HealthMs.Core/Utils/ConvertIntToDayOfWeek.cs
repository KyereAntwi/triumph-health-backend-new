namespace Triumph.HealthMs.Core.Utils;

public static class ConvertIntToDayOfWeek
{
    public static string Convert(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            0 => "Sunday",
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Value must be between 0 and 6.")
        };
    }
}