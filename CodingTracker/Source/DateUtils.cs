using System.Globalization;

namespace vcesario.CodingTracker;

public static class DateUtils
{
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    public static string ToLongDateStringUs(this DateOnly date)
    {
        var stashedCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        string dateString = date.ToLongDateString();

        CultureInfo.CurrentCulture = stashedCulture;

        return dateString;
    }

    public static int DaysBetween(this DateOnly date, DateOnly otherDate)
    {
        DateTime thisDateTime = new(date, TimeOnly.MinValue);
        DateTime otherDateTime = new(otherDate,TimeOnly.MinValue);
        int daysBetween = (thisDateTime - otherDateTime).Duration().Days;
        return daysBetween;
    }
}