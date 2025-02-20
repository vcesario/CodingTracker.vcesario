using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public class ManageSessionsScreen
{
    enum MenuOption
    {
        Week,
        Month,
        Year,
        All,

        Asc,
        Desc,

        EditSession,
        DeleteSessions,

        Return,
    }

    public void Open()
    {
        Console.Clear();

        Console.WriteLine("Search filter");
        Console.WriteLine();

        var actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuOption>()
                    .Title("Select result range:")
                    .AddChoices([MenuOption.Week, MenuOption.Month, MenuOption.Year, MenuOption.All, MenuOption.Return]));

        if (actionChoice == MenuOption.Return)
        {
            return;
        }

        Console.WriteLine($"Range selected: {actionChoice}");

        DateTime filterStart = default;
        DateTime filterEnd = default;

        if (actionChoice == MenuOption.All)
        {
            filterStart = DateTime.MinValue;
            filterEnd = DateTime.MaxValue;
        }
        else
        {
            UserInputValidator validator = new();

            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter any day within the desired range")
                .Validate(validator.ValidateDateOrReturn));

            if (input.ToLower().Equals("return"))
            {
                return;
            }

            DateOnly date = DateOnly.ParseExact(input, "yyyy-MM-dd");

            if (actionChoice == MenuOption.Week)
            {
                DateOnly sunday = date.AddDays(-(int)date.DayOfWeek);
                DateOnly saturday = sunday.AddDays(6);

                filterStart = new(sunday, TimeOnly.MinValue);
                filterEnd = new(saturday, TimeOnly.MaxValue);
            }
            else if (actionChoice == MenuOption.Month)
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
                    new SelectionPrompt<MenuOption>()
                    .Title("Select an ordering for the results")
                    .AddChoices([MenuOption.Asc, MenuOption.Desc]));
        Console.WriteLine("Ordering selected: " + actionChoice);

        Console.WriteLine();
        Console.WriteLine($"Filter: showing sessions from {DateOnly.FromDateTime(filterStart)} to {DateOnly.FromDateTime(filterEnd)}");

        using (var connection = DataService.OpenConnection())
        {
            List<CodingSession> sessions;
            string sql;

            if (actionChoice == MenuOption.Asc)
            {
                sql = @"
                    SELECT rowid, start_date_time, end_date_time FROM coding_sessions
                    WHERE start_date_time >= @FilterStart AND end_date_time <= @FilterEnd
                    ORDER BY start_date_time ASC";
            }
            else
            {
                sql = @"
                    SELECT rowid, start_date_time, end_date_time FROM coding_sessions
                    WHERE start_date_time >= @FilterStart AND end_date_time <= @FilterEnd
                    ORDER BY start_date_time DESC";
            }
            sessions = connection.Query<CodingSession>(sql, new { FilterStart = filterStart, FilterEnd = filterEnd }).ToList();

            if (sessions.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("No entry found for this filter.");
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
            new SelectionPrompt<MenuOption>()
            .AddChoices([MenuOption.EditSession, MenuOption.DeleteSessions, MenuOption.Return])
        );

        switch (actionChoice)
        {
            case MenuOption.EditSession:
                PromptEditSession();
                break;
        }
    }

    private void PromptEditSession()
    {
        UserInputValidator validator = new();
        CodingSession session;

        var input = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the ID of the session to edit")
            .Validate(validator.ValidateLongReturn)
        );

        if (input.ToLower().Equals("return"))
        {
            return;
        }

        long id = long.Parse(input);

        using (var connection = DataService.OpenConnection())
        {
            string sql = "SELECT rowid, start_date_time, end_date_time FROM coding_sessions WHERE rowid=@Id";
            session = connection.QueryFirst<CodingSession>(sql, new { Id = id });

            if (session == null)
            {
                Console.WriteLine("No entry found with this Id.");
                return;
            }
        }

        Console.Clear();
        Console.WriteLine("Edit session");
        Console.WriteLine($"#{session.Id,-6} {session.Start}\t{session.End}");

        var startTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                "Enter the new start date time" + $" [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
                + "\n  > ")
            .Validate(validator.ValidateDateTimeOrReturn)
        );

        if (startTimeInput.ToLower().Equals("return"))
        {
            return;
        }

        var endTimeInput = AnsiConsole.Prompt(
            new TextPrompt<string>(
                "Enter the new end date time" + $" [grey]({ApplicationTexts.USERINPUT_DATETIMEHELPER})[/]"
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
                            SET start_date_time=@Start, end_date_time=@End
                            WHERE rowid=@Id";
            connection.Execute(sql, new { Start = startDateTime, End = endDateTime, Id = id });
        }

        Console.WriteLine();
        Console.WriteLine("Coding session updated.");
        Console.ReadLine();
    }
}