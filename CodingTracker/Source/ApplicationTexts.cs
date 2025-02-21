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
    
    public const string MANAGESESSIONS_HEADER = "Search filter";
    public const string MANAGESESSIONS_PROMPT_RESULTRANGE = "Select result range:";
    public const string MANAGESESSIONS_PROMPT_RESULTRANGE_LOG = "Range selected:";
    public const string MANAGESESSIONS_PROMPT_DAYRANGE = "Enter any day within the desired range";
    public const string MANAGESESSIONS_PROMPT_ORDERING = "Select an ordering for the results";
    public const string MANAGESESSIONS_PROMPT_ORDERING_LOG = "Ordering selected:";
    public const string MANAGESESSIONS_FILTERINFO = "Filter: showing sessions from {0} to {1}";
    public const string MANAGESESSIONS_NOENTRIES = "No entry found for this filter.";
    public const string MANAGESESSIONS_EDIT = "Enter the ID of the session to edit:";
    public const string MANAGESESSIONS_NOENTRIES_ID = "No entry found with this Id.";
    public const string MANAGESESSIONS_EDIT_HEADER = "Edit session";

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