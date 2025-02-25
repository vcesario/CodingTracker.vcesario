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

            Console.WriteLine("View coding goal");

            CodingGoal? goal;
            using (var connection = DataService.OpenConnection())
            {
                string sql = "SELECT value, start_date, due_date FROM coding_goal";
                goal = connection.QueryFirstOrDefault<CodingGoal>(sql);
            }

            Console.WriteLine();
            if (goal == null)
            {
                Console.WriteLine("No coding goal defined.");
            }
            else if (goal.DueDate < DateOnly.FromDateTime(DateTime.Today))
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
                .AddChoices([ManageGoalsOption.NewGoal, ManageGoalsOption.Return])
                .UseConverter((option) =>
                {
                    switch (option)
                    {
                        case ManageGoalsOption.NewGoal:
                            return "Define new goal";
                        case ManageGoalsOption.Return:
                            return "Return";
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
        Console.WriteLine($"Your goal of achieving {goal.Value} hours of coding between {goal.StartDate.ToLongDateStringUs()}"
                            + $" and {goal.DueDate.ToLongDateStringUs()} has expired!");

        uint current = 0;
        float percent = 0;
        // ...
        Console.WriteLine();
        Console.Write("  " + $"Total progress: {current}/{goal.Value} ({percent}%)");

        if (current >= goal.Value)
        {
            Console.WriteLine(" " + "You reached your goal! Congrats!");
        }
        else
        {
            Console.WriteLine(" " + "You didn't reach your goal. Realign your expectations and don't give up. You got this!");
        }
    }


    private void PrintCurrentGoal(CodingGoal goal)
    {
        Console.WriteLine($"Current goal: Achieve {goal.Value} hours of coding by {goal.DueDate.ToLongDateStringUs()}.");

        uint current = 0;
        float percent = 0;
        // ...
        Console.WriteLine();
        Console.Write("  " + $"Total progress: {current}/{goal.Value} ({percent}%)");

        if (current >= goal.Value)
        {
            Console.WriteLine(" " + "You reached your goal! Congrats!");
        }
        else
        {
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("To reach your goal by then, you need to code at least");
        uint dailyTotal = 0;
        // ...
        Console.WriteLine("  " + $"{dailyTotal} hours per day (counting today).");

        uint dailyCurrent = 0;
        float dailyPercent = 0;
        // ...
        Console.WriteLine();
        Console.Write("  " + $"Today's progress: {dailyCurrent}/{dailyTotal} ({dailyPercent}%)");
        if (dailyCurrent >= dailyTotal)
        {
            Console.WriteLine("  " + "You're good for today. Great job!");
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
            new TextPrompt<string>("Set a date for your new goal [grey](yyyy-MM-dd)[/]: ")
            .Validate(validator.ValidateFutureDateOrReturn));

        if (dateInput.ToLower().Equals("return"))
        {
            return;
        }

        var valueInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Set an amount of hours to achieve by the chosen date: ")
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

        Console.WriteLine("New coding goal defined.");
        Console.ReadLine();
    }
}