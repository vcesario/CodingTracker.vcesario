using System.Data.SQLite;
using Spectre.Console;

namespace vcesario.CodingTracker;

public static class MainApplication
{
    public static void Run()
    {
        // create database and tables here?
        DataService.Initialize();
        // ---

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
                case MainMenuOption.LogSessionManually:
                    OpenLogSessionScreen();
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

    private static void OpenLogSessionScreen()
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

        DataService.InsertSession(startDateTime, endDateTime);

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

            DataService.InsertSession(startDateTime, endDateTime);

            Console.WriteLine($"{startDateTime}\t{endDateTime}");
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.RANDOMDATA_CREATED);
        Console.ReadLine();
    }
}