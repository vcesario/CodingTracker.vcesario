using Spectre.Console;

namespace vcesario.CodingTracker;

public static class MainApplication
{
    public static void Run()
    {
        bool choseExitApp = false;
        MainMenuOption actionChoice;
        do
        {
            actionChoice = AnsiConsole.Prompt(
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
        var validator = new UserInputValidator();
        
        var startTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                ApplicationTexts.LOGSESSIONPROMPT_STARTDATETIME + $" [grey]{ApplicationTexts.USERINPUT_DATETIMEHELPER}[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTime)
        );

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                ApplicationTexts.LOGSESSIONPROMPT_ENDDATETIME + $" [grey]{ApplicationTexts.USERINPUT_DATETIMEHELPER}[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTime)
        );

        DateTime startDateTime = DateTime.Parse(startTimeInput);
        DateTime endDateTime = DateTime.Parse(endTimeInput);

        // to be continued...
        // instantiate CodingSession
        // check if valid or invalid
    }
}