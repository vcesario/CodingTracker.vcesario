using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public class ManageGoalsScreen
{
    public enum ManageGoalsOption
    {
        NewGoal,
        Return
    }

    public void Open()
    {
        bool choseReturn = false;
        do
        {
            Console.Clear();

            Console.WriteLine(ApplicationTexts.GOAL_HEADER);

            CodingGoal? goal;
            using (var connection = DataService.OpenConnection())
            {
                string sql = "SELECT value, start_date, due_date FROM coding_goal";
                goal = connection.QueryFirstOrDefault<CodingGoal>(sql);
            }

            Console.WriteLine();
            if (goal == null)
            {
                Console.WriteLine(ApplicationTexts.GOAL_UNDEFINED);
            }
            else if (goal.DueDate < DateUtils.Today)
            {
                PrintPastGoal(goal);
            }
            else
            {
                PrintCurrentGoal(goal);
            }

            Console.WriteLine();
            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<ManageGoalsOption>()
                .Title(ApplicationTexts.GENERIC_PROMPT_ACTION)
                .AddChoices([ManageGoalsOption.NewGoal, ManageGoalsOption.Return])
                .UseConverter((option) =>
                {
                    switch (option)
                    {
                        case ManageGoalsOption.NewGoal:
                            return ApplicationTexts.GOAL_MENUOPTION_NEWGOAL;
                        case ManageGoalsOption.Return:
                            return ApplicationTexts.GENERICMENUOPTION_RETURN;
                        default:
                            return ApplicationTexts.TEXT_UNDEFINED;
                    }
                })
            );

            switch (actionChoice)
            {
                case ManageGoalsOption.NewGoal:
                    PromptNewGoal();
                    break;
                case ManageGoalsOption.Return:
                default:
                    choseReturn = true;
                    break;
            }
        }
        while (!choseReturn);
    }

    private void PrintPastGoal(CodingGoal goal)
    {
        Console.WriteLine(string.Format(ApplicationTexts.GOAL_INFO_PASTGOAL, goal.Value, goal.StartDate.ToLongDateStringUs(), goal.DueDate.ToLongDateStringUs()));

        uint current = GetTotalHoursBetweenDates(goal.StartDate, goal.DueDate);
        float percent = (float)current / goal.Value * 100;
        Console.WriteLine();
        Console.Write("  " + string.Format(ApplicationTexts.GOAL_INFO_TOTALPROGRESS, current, goal.Value, percent));

        if (current >= goal.Value)
        {
            Console.WriteLine(" " + ApplicationTexts.GOAL_PRAISE_SUCCESS);
        }
        else
        {
            Console.WriteLine(" " + ApplicationTexts.GOAL_EXPIRED_FAIL);
        }
    }


    private void PrintCurrentGoal(CodingGoal goal)
    {
        Console.WriteLine(string.Format(ApplicationTexts.GOAL_INFO_CURRENTGOAL, goal.Value, goal.DueDate.ToLongDateStringUs()));

        uint current = GetTotalHoursBetweenDates(goal.StartDate, goal.DueDate);
        float percent = (float)current / goal.Value * 100;
        Console.WriteLine();
        Console.Write("  " + string.Format(ApplicationTexts.GOAL_INFO_TOTALPROGRESS, current, goal.Value, percent));

        if (current >= goal.Value)
        {
            Console.WriteLine(" " + ApplicationTexts.GOAL_PRAISE_SUCCESS);
        }
        else
        {
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.GOAL_INFO_CURRENTGOAL_ATLEAST);

        int remainingDays = goal.DueDate.DaysBetween(DateUtils.Today) + 1;
        uint totalBeforeToday = GetTotalHoursBetweenDates(goal.StartDate, DateUtils.Today.AddDays(-1));
        uint remainingHours = goal.Value - totalBeforeToday;
        float dailyTotal = (float)remainingHours / remainingDays;
        Console.WriteLine("  " + string.Format(ApplicationTexts.GOAL_INFO_CURRENTGOAL_DAILYTOTAL, dailyTotal));

        uint dailyCurrent = GetTotalHoursBetweenDates(DateUtils.Today, DateUtils.Today);
        float dailyPercent = dailyCurrent / dailyTotal * 100;
        Console.WriteLine();
        Console.Write("  " + string.Format(ApplicationTexts.GOAL_INFO_CURRENTGOAL_DAILYPROGRESS, dailyCurrent, dailyTotal, dailyPercent));
        if (dailyCurrent >= dailyTotal)
        {
            Console.WriteLine("  " + ApplicationTexts.GOAL_PRAISE_SUCCESSTODAY);
        }
        else
        {
            Console.WriteLine();
        }
    }

    private void PromptNewGoal()
    {
        UserInputValidator validator = new();

        var dateInput = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.GOAL_PROMPT_NEWDATE)
            .Validate(validator.ValidateFutureDateOrReturn));

        if (dateInput.ToLower().Equals("return"))
        {
            return;
        }

        var valueInput = AnsiConsole.Prompt(
            new TextPrompt<string>(ApplicationTexts.GOAL_PROMPT_HOURS)
            .Validate(validator.ValidatePositiveIntOrReturn));

        if (valueInput.ToLower().Equals("return"))
        {
            return;
        }

        DateTime today = DateTime.Today;
        DateTime dueDate = DateTime.Parse(dateInput);
        int value = int.Parse(valueInput);

        using (var connection = DataService.OpenConnection())
        {
            string sql = "DELETE FROM coding_goal";
            connection.Execute(sql);

            sql = @"INSERT INTO coding_goal (value, start_date, due_date)
                    VALUES (@Value, @StartDate, @DueDate)";
            connection.Execute(sql, new { Value = value, StartDate = today, DueDate = dueDate });
        }

        Console.WriteLine(ApplicationTexts.GOAL_NEWDEFINED);
        Console.ReadLine();
    }

    private uint GetTotalHoursBetweenDates(DateOnly startDate, DateOnly endDate)
    {
        TimeSpan total = TimeSpan.Zero;
        DateTime filterStart = new(startDate, TimeOnly.MinValue);
        DateTime filterEnd = new(endDate, TimeOnly.MaxValue);

        using (var connection = DataService.OpenConnection())
        {
            List<CodingSession> sessions;
            string sql = @"SELECT rowid, start_date, end_date FROM coding_sessions
                            WHERE start_date >= @FilterStart AND end_date <= @FilterEnd";
            sessions = connection.Query<CodingSession>(sql, new { FilterStart = filterStart, FilterEnd = filterEnd }).ToList();

            foreach (CodingSession session in sessions)
            {
                total += session.GetDuration();
            }
        }
        
        uint intTotal = (uint)total.TotalHours;
        return intTotal;
    }
}