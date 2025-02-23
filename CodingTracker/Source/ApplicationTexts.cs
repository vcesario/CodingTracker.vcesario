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
    public const string SESSION_DELETED = "Coding session deleted.";
    public const string SESSION_UPDATED = "Coding session updated.";
    public const string RANDOMDATA_CREATED = "Random data created.";
    public const string TRACKSESSION_INPROGRESS = "Coding session in progress...";
    public const string TRACKSESSION_CONCLUDEHELPER = "(Press 'Enter' to conclude session.)";
    public const string TRACKSESSION_DISCARDHELPER = "(Press 'Esc' to discard session.)";

    public const string MANAGESESSIONSOPTION_ASC = "From earliest";
    public const string MANAGESESSIONSOPTION_DESC = "From latest";
    public const string MANAGESESSIONSOPTION_EDITSESSION = "Edit session";
    public const string MANAGESESSIONSOPTION_DELETESESSIONS = "Delete sessions";
    public const string MANAGESESSIONSOPTION_DELETEID = "Delete single ID";
    public const string MANAGESESSIONSOPTION_DELETEIDRANGE = "Delete ID range";
    public const string MANAGESESSIONSOPTION_DELETEALL = "Delete all sessions";

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
    public const string MANAGESESSIONS_PROMPT_EDITSTART = "Enter the new start date time";
    public const string MANAGESESSIONS_PROMPT_EDITEND = "Enter the new end date time";

    public const string MANAGESESSIONS_DELETE = "Enter the ID of the session to delete:";
    public const string MANAGESESSIONS_DELETE_LOW = "Enter the lowest ID of the range to delete:";
    public const string MANAGESESSIONS_DELETE_HIGH = "Enter the highest ID of the range to delete:";
    public const string MANAGESESSIONS_PROMPT_DELETE = "Are you sure you want to delete session #{0}?";
    public const string MANAGESESSIONS_PROMPT_DELETERANGE = "Are you sure you want to delete all sessions between #{0} and #{1}?";
    public const string MANAGESESSIONS_PROMPT_DELETEALL = "Are you sure you want to delete all sessions?";
    public const string MANAGESESSIONS_DELETE_CANCELED = "Deletion canceled.";
    public const string CONFIRM_AGAIN = "Are you REALLY sure?";

    public const string REPORT_HEADER = "Report";
    public const string REPORT_FIRSTSESSION = "Your first session was on {0}.";
    public const string REPORT_FOLLOWINGDAYS = "Over the following {0} days, you accumulated:";
    public const string REPORT_SESSIONCOUNT = "{0} coding sessions,";
    public const string REPORT_TOTAL_HMS = "a total of {0} hours, {1} minutes and {2} seconds of coding,";
    public const string REPORT_TOTAL_MS = "a total of {0} minutes and {1} seconds of coding,";
    public const string REPORT_TOTAL_S = "a total of {0} seconds of coding,";
    public const string REPORT_AVERAGE_HMS = "and an average of {0} hours, {1} minutes and {2} seconds of coding per session.";
    public const string REPORT_AVERAGE_MS = "and an average of {0} minutes and {1} seconds of coding per session.";
    public const string REPORT_AVERAGE_S = "and an average of {0} seconds of coding per session.";
    public const string REPORT_END = "-- END OF REPORT --";

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

    public static string ConvertManageSessionsOption(ManageSessionsOption option)
    {
        switch (option)
        {
            case ManageSessionsOption.Week:
                return option.ToString();
            case ManageSessionsOption.Month:
                return option.ToString();
            case ManageSessionsOption.Year:
                return option.ToString();
            case ManageSessionsOption.All:
                return option.ToString();
            case ManageSessionsOption.Asc:
                return MANAGESESSIONSOPTION_ASC;
            case ManageSessionsOption.Desc:
                return MANAGESESSIONSOPTION_DESC;
            case ManageSessionsOption.EditSession:
                return MANAGESESSIONSOPTION_EDITSESSION;
            case ManageSessionsOption.DeleteSessions:
                return MANAGESESSIONSOPTION_DELETESESSIONS;
            case ManageSessionsOption.DeleteId:
                return MANAGESESSIONSOPTION_DELETEID;
            case ManageSessionsOption.DeleteIdRange:
                return MANAGESESSIONSOPTION_DELETEIDRANGE;
            case ManageSessionsOption.DeleteAll:
                return MANAGESESSIONSOPTION_DELETEALL;
            case ManageSessionsOption.Return:
                return option.ToString();
            default:
                return TEXT_UNDEFINED;
        }
    }
}