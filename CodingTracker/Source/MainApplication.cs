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

        // using (var connection = DataService.GetConnection())
        // {
        //     // check if session overlaps
        // }

        using (var connection = DataService.OpenConnection())
        {
            SQLiteCommand insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"INSERT INTO coding_sessions (start_date_time, end_date_time)
                                            VALUES (@StartDateTime, @EndDateTime)";
            insertCommand.Parameters.AddWithValue("@StartDateTime", startDateTime);
            insertCommand.Parameters.AddWithValue("@EndDateTime", endDateTime);

            insertCommand.ExecuteNonQuery();
        }

        Console.WriteLine("Coding session created.");
        Console.ReadLine();
    }
}