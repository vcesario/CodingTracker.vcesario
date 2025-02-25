using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public class ManageSessionsScreen
{
    public void Open()
    {
        Console.Clear();

        Console.WriteLine(ApplicationTexts.MANAGESESSIONS_HEADER);
        Console.WriteLine();

        ManageSessionsOption actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<ManageSessionsOption>()
                    .Title(ApplicationTexts.MANAGESESSIONS_PROMPT_RESULTRANGE)
                    .AddChoices([ManageSessionsOption.Week, ManageSessionsOption.Month, ManageSessionsOption.Year, ManageSessionsOption.All, ManageSessionsOption.Return])
                    .UseConverter(ApplicationTexts.ConvertManageSessionsOption));

        if (actionChoice == ManageSessionsOption.Return)
        {
            return;
        }

        Console.WriteLine($"{ApplicationTexts.MANAGESESSIONS_PROMPT_RESULTRANGE_LOG} {actionChoice}");

        DateTime filterStart = default;
        DateTime filterEnd = default;

        if (actionChoice == ManageSessionsOption.All)
        {
            filterStart = DateTime.MinValue;
            filterEnd = DateTime.MaxValue;
        }
        else
        {
            UserInputValidator validator = new();

            var input = AnsiConsole.Prompt(
                new TextPrompt<string>(ApplicationTexts.MANAGESESSIONS_PROMPT_DAYRANGE)
                .Validate(validator.ValidateDateOrReturn));

            if (input.ToLower().Equals("return"))
            {
                return;
            }

            DateOnly date = DateOnly.ParseExact(input, "yyyy-MM-dd");

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
            else // Year
            {
                int year = date.Year;

                DateOnly first = new(year, 1, 1);
                DateOnly last = new(year, 12, DateTime.DaysInMonth(year, 12));

                filterStart = new(first, TimeOnly.MinValue);
                filterEnd = new(last, TimeOnly.MaxValue);
            }
        }

        actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<ManageSessionsOption>()
                    .Title(ApplicationTexts.MANAGESESSIONS_PROMPT_ORDERING)
                    .AddChoices([ManageSessionsOption.Asc, ManageSessionsOption.Desc])
                    .UseConverter(ApplicationTexts.ConvertManageSessionsOption));
        Console.WriteLine($"{ApplicationTexts.MANAGESESSIONS_PROMPT_ORDERING_LOG} {actionChoice}");

        Console.WriteLine();
        Console.WriteLine(string.Format(ApplicationTexts.MANAGESESSIONS_FILTERINFO, DateOnly.FromDateTime(filterStart), DateOnly.FromDateTime(filterEnd)));

        using (var connection = DataService.OpenConnection())
        {
            List<CodingSession> sessions;
            string sql;

            if (actionChoice == ManageSessionsOption.Asc)
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

            if (sessions.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine(ApplicationTexts.MANAGESESSIONS_NOENTRIES);
            }
            else
            {
                Console.WriteLine();
                foreach (var session in sessions)
                {
                    Console.WriteLine($"#{session.Id,-6} {session.Start}\t{session.End}");
                }
            }
        }

        Console.WriteLine();
        actionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<ManageSessionsOption>()
            .AddChoices([ManageSessionsOption.EditSession, ManageSessionsOption.DeleteSessions, ManageSessionsOption.Return])
            .UseConverter(ApplicationTexts.ConvertManageSessionsOption)
        );

        switch (actionChoice)
        {
            case ManageSessionsOption.EditSession:
                PromptEditSession();
                break;

            case ManageSessionsOption.DeleteSessions:
                PromptDeleteSessions();
                break;
        }
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
        Console.WriteLine();
        Console.WriteLine($"#{session.Id,-6} {session.Start}\t{session.End}");

        var startTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                $"{ApplicationTexts.MANAGESESSIONS_PROMPT_EDITSTART} [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (startTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                $"{ApplicationTexts.MANAGESESSIONS_PROMPT_EDITEND} [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (endTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        DateTime startDateTime = DateTime.Parse(startTimeInput);
        DateTime endDateTime = DateTime.Parse(endTimeInput);

        // check session overlap
        // ...

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
            .UseConverter(ApplicationTexts.ConvertManageSessionsOption)
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
            new ConfirmationPrompt(string.Format(ApplicationTexts.MANAGESESSIONS_PROMPT_DELETERANGE, input, input2))
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
}