using System.Diagnostics;
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
                    ManageSessions();
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

    private static void ManageSessions()
    {
        Console.Clear();

        Console.WriteLine("Search filter");

        var actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Select result range:")
                    .AddChoices(["Week", "Month", "Year", "All", "Return"]));

        if (actionChoice.Equals("Return"))
        {
            return;
        }

        Console.WriteLine($"Range selected: {actionChoice}");

        DateTime filterStart = default;
        DateTime filterEnd = default;

        if (actionChoice.Equals("All"))
        {
            filterStart = DateTime.MinValue;
            filterEnd = DateTime.MaxValue;
        }
        else
        {
            UserInputValidator validator = new();

            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter any day within the desired range")
                .Validate(validator.ValidateDateOrReturn));

            if (input.ToLower().Equals("return"))
            {
                return;
            }

            DateOnly date = DateOnly.ParseExact(input, "yyyy-MM-dd");

            if (actionChoice.Equals("Week"))
            {
                DateOnly sunday = date.AddDays(-(int)date.DayOfWeek);
                DateOnly saturday = sunday.AddDays(6);

                filterStart = new(sunday, TimeOnly.MinValue);
                filterEnd = new(saturday, TimeOnly.MaxValue);
            }
            else if (actionChoice.Equals("Month"))
            {
                int month = date.Month;
                int year = date.Year;

                DateOnly first = new(year, month, 1);
                DateOnly last = new(year, month, DateTime.DaysInMonth(year, month));

                filterStart = new(first, TimeOnly.MinValue);
                filterEnd = new(last, TimeOnly.MaxValue);
            }
            else // Year
            {
                int year = date.Year;

                DateOnly first = new(year, 1, 1);
                DateOnly last = new(year, 12, DateTime.DaysInMonth(year, 12));

                filterStart = new(first, TimeOnly.MinValue);
                filterEnd = new(last, TimeOnly.MaxValue);
            }
        }

        actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Select an ordering for the results")
                    .AddChoices(["From oldest", "From newest"]));
        Console.WriteLine("Ordering selected: " + actionChoice);

        Console.WriteLine();
        Console.WriteLine($"Filter: showing sessions from {DateOnly.FromDateTime(filterStart)} to {DateOnly.FromDateTime(filterEnd)}");
        Console.WriteLine();

        using (var connection = DataService.OpenConnection())
        {
            List<CodingSession> sessions;
            string selectCommand;

            if (actionChoice.Equals("From oldest"))
            {
                selectCommand = @"
                    SELECT rowid, start_date_time, end_date_time FROM coding_sessions
                    WHERE start_date_time >= @FilterStart AND end_date_time <= @FilterEnd
                    ORDER BY start_date_time ASC";
            }
            else
            {
                selectCommand = @"
                    SELECT rowid, start_date_time, end_date_time FROM coding_sessions
                    WHERE start_date_time >= @FilterStart AND end_date_time <= @FilterEnd
                    ORDER BY start_date_time DESC";
            }
            sessions = connection.Query<CodingSession>(selectCommand, new { FilterStart = filterStart, FilterEnd = filterEnd }).ToList();

            foreach (var session in sessions)
            {
                Console.WriteLine($"[{session.Id}] {session.Start} - {session.End}");
            }
        }
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