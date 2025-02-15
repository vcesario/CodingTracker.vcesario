using Spectre.Console;

namespace vcesario.CodingTracker;

public static class ApplicationTexts
{
    public const string MAINMENU_PROMPT = "What do you want to do?";
    public const string LOGSESSIONPROMPT_STARTDATETIME = "Enter the start date and time";
    public const string LOGSESSIONPROMPT_ENDDATETIME = "Enter the end date and time";
    public const string USERINPUT_DATETIMEHELPER = "yyyy-MM-dd hh:mm:ss";
    public const string USERINPUT_DATETIMEERROR = "Couldn't parse DateTime. Use providede template.";
    private const string TEXT_UNDEFINED = "Text undefined";

    public static string ConvertMainMenuOption(MainMenuOption option)
    {
        switch (option)
        {
            case MainMenuOption.StartNewSession:
                return "Start new session";
            case MainMenuOption.LogSessionManually:
                return "Enter session manually";
            case MainMenuOption.ViewReport:
                return "View report";
            case MainMenuOption.ViewGoals:
                return "View goals";
            case MainMenuOption.FillWithRandomData:
                return Markup.Escape("[DEBUG] Fill with random data");
            case MainMenuOption.ExitApplication:
                return "Exit application";
            default:
                return TEXT_UNDEFINED;
        }
    }
}