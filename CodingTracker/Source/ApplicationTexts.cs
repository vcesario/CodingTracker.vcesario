using Spectre.Console;

namespace vcesario.CodingTracker;

public static class ApplicationTexts
{
    public const string MAINMENU_PROMPT = "What do you want to do?";
    public const string MAINMENUOPTION_STARTNEWSESSION = "Start new session";
    public const string MAINMENUOPTION_LOGSESSION = "Enter session manually";
    public const string MAINMENUOPTION_MANAGESESSIONS = "Manage sessions";
    public const string MAINMENUOPTION_VIEWREPORT = "View report";
    public const string MAINMENUOPTION_VIEWGOALS = "View goals";
    public const string MAINMENUOPTION_FILLWITHRANDOM = "[DEBUG] Fill with random data";
    public const string MAINMENUOPTION_EXIT = "Exit application";
    public const string LOGSESSIONPROMPT_STARTDATETIME = "Enter the start date and time";
    public const string LOGSESSIONPROMPT_ENDDATETIME = "Enter the end date and time";
    public const string CODINGSESSION_NOTACCEPTED = "This coding session is not acceptable.";
    public const string USERINPUT_DATETIMEHELPER = "yyyy-MM-dd hh:mm:ss";
    public const string USERINPUT_DATETIMEERROR = "Couldn't parse DateTime. Use provided template.";
    public const string SESSION_CREATED = "Coding session created.";
    public const string SESSION_DISCARDED = "Coding session discarded.";
    public const string RANDOMDATA_CREATED = "Random data created.";
    public const string TRACKSESSION_INPROGRESS = "Coding session in progress...";
    public const string TRACKSESSION_CONCLUDEHELPER = "(Press 'Enter' to conclude session.)";
    public const string TRACKSESSION_DISCARDHELPER = "(Press 'Esc' to discard session.)";

    private const string TEXT_UNDEFINED = "Text undefined";

    public static string ConvertMainMenuOption(MainMenuOption option)
    {
        switch (option)
        {
            case MainMenuOption.StartNewSession:
                return MAINMENUOPTION_STARTNEWSESSION;
            case MainMenuOption.LogSessionManually:
                return MAINMENUOPTION_LOGSESSION;
            case MainMenuOption.ManageSessions:
                return MAINMENUOPTION_MANAGESESSIONS;
            case MainMenuOption.ViewReport:
                return MAINMENUOPTION_VIEWREPORT;
            case MainMenuOption.ViewGoals:
                return MAINMENUOPTION_VIEWGOALS;
            case MainMenuOption.FillWithRandomData:
                return Markup.Escape(MAINMENUOPTION_FILLWITHRANDOM);
            case MainMenuOption.ExitApplication:
                return MAINMENUOPTION_EXIT;
            default:
                return TEXT_UNDEFINED;
        }
    }
}