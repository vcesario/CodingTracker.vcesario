using System.Diagnostics;
using System.Globalization;
using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public static class MainApplication
{
    public static void Run()
    {
        DataService.Initialize();

        bool choseExitApp = false;
        do
        {
            Console.Clear();

            MainMenuOption actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<MainMenuOption>()
                    .Title(ApplicationTexts.MAINMENU_PROMPT)
                    .AddChoices(Enum.GetValues<MainMenuOption>())
                    .UseConverter(ApplicationTexts.ConvertMainMenuOption));

            switch (actionChoice)
            {
                case MainMenuOption.StartNewSession:
                    TrackSession();
                    break;
                case MainMenuOption.LogSessionManually:
                    EnterManualSession();
                    break;
                case MainMenuOption.ManageSessions:
                    new ManageSessionsScreen().Open();
                    break;
                case MainMenuOption.ViewReport:
                    ViewReport();
                    break;
                case MainMenuOption.ViewGoals:
                    new ManageGoalsScreen().Open();
                    break;
                case MainMenuOption.FillWithRandomData:
                    FillWithRandomData();
                    break;
                default:
                    choseExitApp = true;
                    break;
            }
        }
        while (!choseExitApp);
    }

    private static void TrackSession()
    {
        Stopwatch stopwatch = new();
        DateTime startDateTime = DateTime.Now;
        Task<ConsoleKeyInfo> keyReader = Task.Run(ReadUserKey);

        stopwatch.Start();

        while (!keyReader.IsCompleted)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine($"  {ApplicationTexts.TRACKSESSION_INPROGRESS}");
            Console.WriteLine($"\t{stopwatch.Elapsed:hh\\:mm\\:ss}");
            Console.WriteLine();
            AnsiConsole.MarkupLine($"  [grey]{ApplicationTexts.TRACKSESSION_CONCLUDEHELPER}[/]");
            AnsiConsole.MarkupLine($"  [grey]{ApplicationTexts.TRACKSESSION_DISCARDHELPER}[/]");
            Console.WriteLine();
            Thread.Sleep(500);
        }

        stopwatch.Stop();

        if (keyReader.Result.Key != ConsoleKey.Enter)
        {
            Console.WriteLine(ApplicationTexts.SESSION_DISCARDED);
            Console.ReadLine();
            return;
        }

        DateTime endDateTime = DateTime.Now;
        CodingSession codingSession = new(startDateTime, endDateTime);

        // check if session overlaps
        // ...

        DataService.InsertSession(codingSession);

        Console.WriteLine($"{ApplicationTexts.SESSION_CREATED}\n  {startDateTime}\t{endDateTime}");
        Console.ReadLine();

        ConsoleKeyInfo ReadUserKey()
        {
            ConsoleKeyInfo keyInfo = default;
            bool pressedAcceptedKey = false;

            while (!pressedAcceptedKey)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter
                || keyInfo.Key == ConsoleKey.Escape)
                {
                    pressedAcceptedKey = true;
                }
            }

            return keyInfo;
        }
    }

    private static void EnterManualSession()
    {
        Console.Clear();

        var validator = new UserInputValidator();

        var startTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                ApplicationTexts.LOGSESSIONPROMPT_STARTDATETIME + $" [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (startTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                ApplicationTexts.LOGSESSIONPROMPT_ENDDATETIME + $" [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (endTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        DateTime startDateTime = DateTime.Parse(startTimeInput);
        DateTime endDateTime = DateTime.Parse(endTimeInput);

        CodingSession session = new(startDateTime, endDateTime);
        if (!session.Validate())
        {
            Console.WriteLine(ApplicationTexts.CODINGSESSION_NOTACCEPTED);
            Console.ReadLine();
            return;
        }

        // check if session overlaps
        // ...
        DataService.InsertSession(session);

        Console.WriteLine(ApplicationTexts.SESSION_CREATED);
        Console.ReadLine();
    }

    private static void ViewReport()
    {
        Console.Clear();

        // Your first coding session was on DD/MM/YYYY.
        // Over the following D days, you accumulated:
        //   N coding sessions
        //   and a total of X hours, Y minutes and Z seconds of coding,
        //   with an average of A hours, B minutes and C seconds of coding per session.

        List<CodingSession> sessions;
        DateTime filterStart = DateTime.MinValue;
        DateTime filterEnd = DateTime.MaxValue;

        using (var connection = DataService.OpenConnection())
        {
            string sql = @"SELECT rowid, start_date_time, end_date_time FROM coding_sessions
                        WHERE start_date_time >= @FilterStart AND end_date_time <= @FilterEnd
                        ORDER BY start_date_time ASC";
            sessions = connection.Query<CodingSession>(sql, new { FilterStart = filterStart, FilterEnd = filterEnd }).ToList();
        }

        if (sessions.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_NOENTRIES);
            Console.ReadLine();
            return;
        }

        Console.WriteLine(ApplicationTexts.REPORT_HEADER);
        Console.WriteLine();

        var stashedCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        DateOnly firstDate = DateOnly.FromDateTime(sessions[0].Start);
        Console.WriteLine(string.Format(ApplicationTexts.REPORT_FIRSTSESSION, firstDate.ToLongDateString()));

        CultureInfo.CurrentCulture = stashedCulture;
        Console.ReadLine();

        TimeSpan dayCount = sessions[sessions.Count - 1].End - sessions[0].Start;
        Console.Write(string.Format(ApplicationTexts.REPORT_FOLLOWINGDAYS, dayCount.Days));
        Console.ReadLine();

        Console.Write("  " + string.Format(ApplicationTexts.REPORT_SESSIONCOUNT, sessions.Count));
        Console.ReadLine();

        TimeSpan durationTotal = TimeSpan.Zero;
        foreach (var session in sessions)
        {
            durationTotal += session.GetDuration();
        }

        if (durationTotal.Hours > 0)
        {
            int daysInHours = durationTotal.Days * 24;
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_TOTAL_HMS, durationTotal.Hours + daysInHours, durationTotal.Minutes, durationTotal.Seconds));
        }
        else if (durationTotal.Minutes > 0)
        {
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_TOTAL_MS, durationTotal.Minutes, durationTotal.Seconds));
        }
        else
        {
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_TOTAL_S, durationTotal.Seconds));
        }
        Console.ReadLine();

        TimeSpan durationAverage = durationTotal / sessions.Count;
        if (durationAverage.Hours > 0)
        {
            int daysInHours = durationAverage.Days * 24;
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_AVERAGE_HMS, durationAverage.Hours + daysInHours, durationAverage.Minutes, durationAverage.Seconds));
        }
        else if (durationAverage.Minutes > 0)
        {
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_AVERAGE_MS, durationAverage.Minutes, durationAverage.Seconds));
        }
        else
        {
            Console.Write("  " + string.Format(ApplicationTexts.REPORT_AVERAGE_S, durationAverage.Seconds));
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.Write($"\t{ApplicationTexts.REPORT_END}");
        Console.ReadLine();
    }

    private static void FillWithRandomData()
    {
        Console.Clear();

        Random random = new();
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        for (int i = 1; i <= 100; i++)
        {
            TimeSpan sessionDuration = TimeSpan.FromMinutes(random.Next(1, 181));
            TimeOnly timeRange = TimeOnly.MaxValue.AddMinutes(-sessionDuration.TotalMinutes);
            TimeOnly randomStartTime = new(
                random.Next(timeRange.Hour + 1),
                random.Next(timeRange.Minute + 1),
                random.Next(timeRange.Second + 1));

            DateOnly day = today.AddDays(-i);

            DateTime startDateTime = new(day, randomStartTime);
            DateTime endDateTime = new(day, randomStartTime.AddMinutes(sessionDuration.TotalMinutes));

            // check if session overlaps
            // ...

            CodingSession session = new(startDateTime, endDateTime);
            DataService.InsertSession(session);

            Console.WriteLine($"  {startDateTime:yyyy-MM-dd HH:mm:ss}\t{endDateTime:yyyy-MM-dd HH:mm:ss}");
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.RANDOMDATA_CREATED);
        Console.ReadLine();
    }
}