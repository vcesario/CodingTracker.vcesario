namespace vcesario.CodingTracker;

public static class ApplicationTexts
{
    public const string MAINMENUOPTION_STARTNEWSESSION = "Start new session";
    public const string MAINMENUOPTION_LOGSESSION = "Enter session manually";
    public const string MAINMENUOPTION_MANAGESESSIONS = "Manage sessions";
    public const string MAINMENUOPTION_VIEWREPORT = "View report";
    public const string MAINMENUOPTION_VIEWGOALS = "View goals";
    public const string MAINMENUOPTION_FILLWITHRANDOM = "[DEBUG] Fill with random data";
    public const string MAINMENUOPTION_EXIT = "Exit application";

    public const string LOGSESSIONPROMPT_STARTDATETIME = "Enter the start date and time";
    public const string LOGSESSIONPROMPT_ENDDATETIME = "Enter the end date and time";

    public const string USERINPUT_DATETIMEERROR = "Couldn't parse DateTime. Use provided template.";
    public const string USERINPUT_DATEERROR = "Couldn't parse Date. Use provided template.";
    public const string USERINPUT_OLDERDATEERROR = "Cannot accept dates older than today.";
    public const string USERINPUT_LONGERROR = "Couldn't parse number. Try again.";

    public const string SESSION_INVALID = "Coding session invalid.";
    public const string SESSION_CREATED = "Coding session created.";
    public const string SESSION_DISCARDED = "Coding session discarded.";
    public const string SESSION_DELETED = "Coding session deleted.";
    public const string SESSION_UPDATED = "Coding session updated.";

    public const string RANDOMDATA_CREATED = "Random data created.";

    public const string TRACKSESSION_INPROGRESS = "Coding session in progress...";
    public const string TRACKSESSION_CONCLUDEHELPER = "Press 'Enter' to conclude session.";
    public const string TRACKSESSION_DISCARDHELPER = "Press 'Esc' to discard session.";

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
    public const string MANAGESESSIONS_PROMPT_DELETERANGE = "Are you sure you want to delete all sessions between {0} and {1}?";
    public const string MANAGESESSIONS_PROMPT_DELETEALL = "Are you sure you want to delete all sessions?";
    public const string MANAGESESSIONS_DELETE_CANCELED = "Deletion canceled.";
    public const string CONFIRM_AGAIN = "Are you REALLY sure?";

    public const string REPORT_HEADER = "View report";
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

    public const string GOAL_HEADER = "View coding goal";
    public const string GOAL_NEWDEFINED = "New coding goal defined.";
    public const string GOAL_UNDEFINED = "No coding goal defined.";
    public const string GOAL_MENUOPTION_NEWGOAL = "Define new goal";
    public const string GOAL_INFO_PASTGOAL = "Your goal of achieving {0} hours of coding between {1} and {2} has expired!";
    public const string GOAL_INFO_TOTALPROGRESS = "Total progress: {0}/{1} ({2}%)";
    public const string GOAL_PRAISE_SUCCESS = "You reached your goal! Congrats!";
    public const string GOAL_EXPIRED_FAIL = "You didn't reach your goal, but don't give up. You got this!";
    public const string GOAL_INFO_CURRENTGOAL = "Current goal: Achieve {0} hours of coding by {1}.";
    public const string GOAL_INFO_CURRENTGOAL_ATLEAST = "To reach your goal by then, you need to code at least";
    public const string GOAL_INFO_CURRENTGOAL_DAILYTOTAL = "{0} hours per day (counting today).";
    public const string GOAL_INFO_CURRENTGOAL_DAILYPROGRESS = "Today's progress: {0}/{1} ({2}%)";
    public const string GOAL_PRAISE_SUCCESSTODAY = "You're good for today. Great job!";
    public const string GOAL_PROMPT_NEWDATE = "Set a date for your new goal";
    public const string GOAL_PROMPT_HOURS = "Set an amount of hours to achieve by the chosen date:";

    public const string DATASERVICE_OVERLAP_INFO = "This new session would overlap the following sessions:";
    public const string DATASERVICE_OVERLAP_PROMPT = "Do you want to delete the sessions above to insert the new one?";

    public const string GENERIC_DB_ERROR = "Error when connecting to database. Try restarting the application.";
    public const string GENERIC_PROMPT_ACTION = "What do you want to do?";
    public const string GENERICMENUOPTION_RETURN = "Return";
    public const string TEXT_UNDEFINED = "Text undefined";
}