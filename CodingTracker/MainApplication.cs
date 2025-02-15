using Spectre.Console;

namespace vcesario.CodingTracker;

public static class MainApplication
{
    public static void Run()
    {
        MainMenuOption actionChoice;
        do
        {
            actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<MainMenuOption>()
                    .Title(ApplicationTexts.MAINMENU_PROMPT)
                    .AddChoices(Enum.GetValues<MainMenuOption>())
                    .UseConverter(ApplicationTexts.ConvertMainMenuOption));
        }
        while(actionChoice != MainMenuOption.ExitApplication);
    }
}