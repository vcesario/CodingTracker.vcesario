using System.Globalization;

namespace vcesario.CodingTracker;

public static class LoggingUtils
{
    public static string ToLongDateStringUs(this DateOnly date)
    {
        var stashedCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        string dateString = date.ToLongDateString();

        CultureInfo.CurrentCulture = stashedCulture;

        return dateString;
    }
}