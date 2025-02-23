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
        Console.Clear();

        Console.WriteLine("View coding goal");

        CodingGoal? goal;
        using (var connection = DataService.OpenConnection())
        {
            string sql = "SELECT value, due_date FROM coding_goal";
            goal = connection.QueryFirstOrDefault<CodingGoal>(sql);
        }

        Console.WriteLine();
        if (goal == null)
        {
            Console.WriteLine("No coding goal defined.");
        }
        else
        {
            PrintGoal(goal);
        }

        Console.WriteLine();
        var actionChoice = AnsiConsole.Prompt(
            new SelectionPrompt<ManageGoalsOption>()
            .AddChoices([ManageGoalsOption.NewGoal, ManageGoalsOption.Return])
        );

        switch (actionChoice)
        {
            case ManageGoalsOption.NewGoal:
                PromptNewGoal();
                break;
            case ManageGoalsOption.Return:
            default:
                break;
        }
    }

    private void PrintGoal(CodingGoal goal)
    {
        Console.WriteLine("Goal was found.");
    }

    private void PromptNewGoal()
    {

    }
}