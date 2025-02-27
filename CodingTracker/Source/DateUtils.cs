using System.Globalization;
using Spectre.Console;

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

    public static void DrawSessionTable(List<CodingSession> sessions)
    {
        Console.WriteLine();

        var table = new Table();

        table.AddColumn(new TableColumn("[yellow]Id[/]").RightAligned());
        table.AddColumn(new TableColumn("[yellow]Start time[/]").Centered());
        table.AddColumn(new TableColumn("[yellow]End time[/]").Centered());

        if (sessions.Count == 0)
        {
            table.AddRow("---", "----------", "----------");
        }
        else
        {
            foreach (var session in sessions)
            {
                string startDate = "[cyan]" + DateOnly.FromDateTime(session.Start.Date) + "[/] ";
                startDate += session.Start.TimeOfDay;

                string endDate = "[cyan]" + DateOnly.FromDateTime(session.End.Date) + "[/] ";
                endDate += session.End.TimeOfDay;

                table.AddRow(session.Id.ToString(), startDate, endDate);
            }
        }

        table.Border = TableBorder.Horizontal;

        AnsiConsole.Write(table);
    }
}