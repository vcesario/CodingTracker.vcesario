using System.Diagnostics;
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

        if (startTimeInput.Equals("return"))
        {
            return;
        }

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                ApplicationTexts.LOGSESSIONPROMPT_ENDDATETIME + $" [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (endTimeInput.Equals("return"))
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