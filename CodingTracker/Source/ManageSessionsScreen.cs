using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public class ManageSessionsScreen
{
    public enum ManageSessionsOption
    {
        Week,
        Month,
        Year,
        All,

        Asc,
        Desc,

        EditSession,
        DeleteSessions,

        DeleteId,
        DeleteIdRange,
        DeleteAll,

        Return,
    }

    public void Open()
    {
        bool choseReturn = false;
        do
        {
            Console.Clear();

            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_HEADER);
            Console.WriteLine();

            if (!PromptSearchFilter(out DateTime filterStart, out DateTime filterEnd, out ManageSessionsOption order))
            {
                return;
            }


            Console.WriteLine();
            Console.WriteLine(string.Format(ApplicationTexts.MANAGESESSIONS_FILTERINFO, DateOnly.FromDateTime(filterStart), DateOnly.FromDateTime(filterEnd)));

            List<CodingSession> sessions;

            using (var connection = DataService.OpenConnection())
            {
                string sql;

                if (order == ManageSessionsOption.Asc)
                {
                    sql = @"SELECT rowid, start_date, end_date FROM coding_sessions
                        WHERE start_date >= @FilterStart AND end_date <= @FilterEnd
                        ORDER BY start_date ASC";
                }
                else
                {
                    sql = @"SELECT rowid, start_date, end_date FROM coding_sessions
                        WHERE start_date >= @FilterStart AND end_date <= @FilterEnd
                        ORDER BY start_date DESC";
                }
                sessions = connection.Query<CodingSession>(sql, new { FilterStart = filterStart, FilterEnd = filterEnd }).ToList();
            }

            DateUtils.DrawSessionTable(sessions);

            Console.WriteLine();
            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<ManageSessionsOption>()
                .Title(ApplicationTexts.GENERIC_PROMPT_ACTION)
                .AddChoices([ManageSessionsOption.EditSession, ManageSessionsOption.DeleteSessions, ManageSessionsOption.Return])
                .UseConverter(ConvertManageSessionsOption)
            );

            switch (actionChoice)
            {
                case ManageSessionsOption.EditSession:
                    PromptEditSession();
                    break;

                case ManageSessionsOption.DeleteSessions:
                    PromptDeleteSessions();
                    break;

                case ManageSessionsOption.Return:
                default:
                    choseReturn = true;
                    break;
            }
        }
        while (!choseReturn);
    }

    private bool PromptSearchFilter(out DateTime filterStart, out DateTime filterEnd, out ManageSessionsOption order)
    {
        filterStart = default;
        filterEnd = default;
        order = default;

        ManageSessionsOption actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<ManageSessionsOption>()
                    .Title(ApplicationTexts.MANAGESESSIONS_PROMPT_RESULTRANGE)
                    .AddChoices([ManageSessionsOption.Week, ManageSessionsOption.Month, ManageSessionsOption.Year, ManageSessionsOption.All, ManageSessionsOption.Return])
                    .UseConverter(ConvertManageSessionsOption));

        if (actionChoice == ManageSessionsOption.Return)
        {
            return false;
        }

        Console.WriteLine($"{ApplicationTexts.MANAGESESSIONS_PROMPT_RESULTRANGE_LOG} {actionChoice}");

        if (actionChoice == ManageSessionsOption.All)
        {
            filterStart = DateTime.MinValue;
            filterEnd = DateTime.MaxValue;
        }
        else
        {
            UserInputValidator validator = new();

            var input = AnsiConsole.Prompt(
                new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_PROMPT_DAYRANGE + "[grey](DD/MM/YYYY)[/]:")
                .Validate(validator.ValidateDateOrReturn));

            if (input.ToLower().Equals("return"))
            {
                return false;
            }

            DateOnly date = DateOnly.ParseExact(input, "dd/MM/yyyy");

            if (actionChoice == ManageSessionsOption.Week)
            {
                DateOnly sunday = date.AddDays(-(int)date.DayOfWeek);
                DateOnly saturday = sunday.AddDays(6);

                filterStart = new(sunday, TimeOnly.MinValue);
                filterEnd = new(saturday, TimeOnly.MaxValue);
            }
            else if (actionChoice == ManageSessionsOption.Month)
            {
                int month = date.Month;
                int year = date.Year;

                DateOnly first = new(year, month, 1);
                DateOnly last = new(year, month, DateTime.DaysInMonth(year, month));

                filterStart = new(first, TimeOnly.MinValue);
                filterEnd = new(last, TimeOnly.MaxValue);
            }
            else
            {
                int year = date.Year;

                DateOnly first = new(year, 1, 1);
                DateOnly last = new(year, 12, DateTime.DaysInMonth(year, 12));

                filterStart = new(first, TimeOnly.MinValue);
                filterEnd = new(last, TimeOnly.MaxValue);
            }
        }

        order = AnsiConsole.Prompt(
                    new SelectionPrompt<ManageSessionsOption>()
                    .Title(ApplicationTexts.MANAGESESSIONS_PROMPT_ORDERING)
                    .AddChoices([ManageSessionsOption.Asc, ManageSessionsOption.Desc])
                    .UseConverter(ConvertManageSessionsOption));
        Console.WriteLine($"{ApplicationTexts.MANAGESESSIONS_PROMPT_ORDERING_LOG} {order}");

        return true;
    }

    private void PromptEditSession()
    {
        UserInputValidator validator = new();
        CodingSession session;

        var input = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_EDIT)
            .Validate(validator.ValidateLongReturn)
        );

        if (input.ToLower().Equals("return"))
        {
            return;
        }

        long id = long.Parse(input);

        using (var connection = DataService.OpenConnection())
        {
            string sql = "SELECT rowid, start_date, end_date FROM coding_sessions WHERE rowid=@Id";
            session = connection.QueryFirst<CodingSession>(sql, new { Id = id });

            if (session == null)
            {
                Console.WriteLine(ApplicationTexts.MANAGESESSIONS_NOENTRIES_ID);
                return;
            }
        }

        Console.Clear();
        Console.WriteLine(ApplicationTexts.MANAGESESSIONS_EDIT_HEADER);
        DateUtils.DrawSessionTable(new() { session });
        Console.WriteLine();

        var startTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_PROMPT_EDITSTART + "[grey](DD/MM/YYYY hh:mm:ss)[/]:")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (startTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_PROMPT_EDITEND + "[grey](DD/MM/YYYY hh:mm:ss)[/]")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (endTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        DateTime startDateTime = DateTime.Parse(startTimeInput);
        DateTime endDateTime = DateTime.Parse(endTimeInput);

        CodingSession dummySession = new(session.Id, startDateTime, endDateTime);
        if (!DataService.PromptSessionOverlap(dummySession))
        {
            Console.WriteLine(ApplicationTexts.SESSION_DISCARDED);
            Console.ReadLine();
            return;
        }

        using (var connection = DataService.OpenConnection())
        {
            string sql = @"UPDATE coding_sessions
                            SET start_date=@Start, end_date=@End
                            WHERE rowid=@Id";
            connection.Execute(sql, new { Start = startDateTime, End = endDateTime, Id = id });
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.SESSION_UPDATED);
        Console.ReadLine();
    }

    private void PromptDeleteSessions()
    {
        var actionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<ManageSessionsOption>()
            .AddChoices([ManageSessionsOption.DeleteId, ManageSessionsOption.DeleteIdRange, ManageSessionsOption.DeleteAll, ManageSessionsOption.Return])
            .UseConverter(ConvertManageSessionsOption)
        );

        switch (actionChoice)
        {
            case ManageSessionsOption.DeleteId:
                PromptDeleteId();
                break;
            case ManageSessionsOption.DeleteIdRange:
                PromptDeleteIdRange();
                break;
            case ManageSessionsOption.DeleteAll:
                PromptDeleteAll();
                break;
            case ManageSessionsOption.Return:
                break;
        }
    }

    private void PromptDeleteId()
    {
        UserInputValidator validator = new();

        var input = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_DELETE)
            .Validate(validator.ValidateLongReturn)
        );

        if (input.ToLower().Equals("return"))
        {
            return;
        }

        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(string.Format(ApplicationTexts.MANAGESESSIONS_PROMPT_DELETE, input))
            {
                DefaultValue = false
            }
        );

        if (!confirmation)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_DELETE_CANCELED);
            Console.ReadLine();

            return;
        }

        long id = long.Parse(input);

        using (var connection = DataService.OpenConnection())
        {
            string sql = "DELETE FROM coding_sessions WHERE rowid=@Id";
            connection.Execute(sql, new { Id = id });
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.SESSION_DELETED);
        Console.ReadLine();
    }

    private void PromptDeleteIdRange()
    {
        UserInputValidator validator = new();

        var input = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_DELETE_LOW)
            .Validate(validator.ValidateLongReturn)
        );

        if (input.ToLower().Equals("return"))
        {
            return;
        }

        var input2 = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_DELETE_HIGH)
            .Validate(validator.ValidateLongReturn)
        );

        if (input2.ToLower().Equals("return"))
        {
            return;
        }

        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(string.Format(ApplicationTexts.MANAGESESSIONS_PROMPT_DELETERANGE, "[red]" + input + "[/]", "[red]" + input2 + "[/]"))
            {
                DefaultValue = false
            }
        );

        if (!confirmation)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_DELETE_CANCELED);
            Console.ReadLine();

            return;
        }

        confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(ApplicationTexts.CONFIRM_AGAIN)
            {
                DefaultValue = false
            }
        );

        if (!confirmation)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_DELETE_CANCELED);
            Console.ReadLine();

            return;
        }

        long id = long.Parse(input);
        long id2 = long.Parse(input2);

        if (id > id2)
        {
            (id, id2) = (id2, id);
        }

        using (var connection = DataService.OpenConnection())
        {
            string sql = "DELETE FROM coding_sessions WHERE rowid>=@Id1 AND rowid<=@Id2";
            connection.Execute(sql, new { Id1 = id, Id2 = id2 });
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.SESSION_DELETED);
        Console.ReadLine();
    }

    private void PromptDeleteAll()
    {
        var confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(ApplicationTexts.MANAGESESSIONS_PROMPT_DELETEALL)
            {
                DefaultValue = false
            }
        );

        if (!confirmation)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_DELETE_CANCELED);
            Console.ReadLine();

            return;
        }

        confirmation = AnsiConsole.Prompt(
            new ConfirmationPrompt(ApplicationTexts.CONFIRM_AGAIN)
            {
                DefaultValue = false
            }
        );

        if (!confirmation)
        {
            Console.WriteLine();
            Console.WriteLine(ApplicationTexts.MANAGESESSIONS_DELETE_CANCELED);
            Console.ReadLine();

            return;
        }

        using (var connection = DataService.OpenConnection())
        {
            string sql = "DELETE FROM coding_sessions";
            connection.Execute(sql);
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.SESSION_DELETED);
        Console.ReadLine();
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
                return ApplicationTexts.MANAGESESSIONSOPTION_ASC;
            case ManageSessionsOption.Desc:
                return ApplicationTexts.MANAGESESSIONSOPTION_DESC;
            case ManageSessionsOption.EditSession:
                return ApplicationTexts.MANAGESESSIONSOPTION_EDITSESSION;
            case ManageSessionsOption.DeleteSessions:
                return ApplicationTexts.MANAGESESSIONSOPTION_DELETESESSIONS;
            case ManageSessionsOption.DeleteId:
                return ApplicationTexts.MANAGESESSIONSOPTION_DELETEID;
            case ManageSessionsOption.DeleteIdRange:
                return ApplicationTexts.MANAGESESSIONSOPTION_DELETEIDRANGE;
            case ManageSessionsOption.DeleteAll:
                return ApplicationTexts.MANAGESESSIONSOPTION_DELETEALL;
            case ManageSessionsOption.Return:
                return ApplicationTexts.GENERICMENUOPTION_RETURN;
            default:
                return ApplicationTexts.TEXT_UNDEFINED;
        }
    }
}