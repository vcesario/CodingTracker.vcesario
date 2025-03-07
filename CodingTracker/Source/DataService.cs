using System.Configuration;
using System.Data.SQLite;
using Dapper;
using Spectre.Console;

namespace vcesario.CodingTracker;

public static class DataService
{
    public static void Initialize()
    {
        using (var connection = OpenConnection())
        {
            string sql = @"CREATE TABLE IF NOT EXISTS coding_sessions(
                                start_date DATE NOT NULL,
                                end_date DATE NOT NULL
                            );
                            
                            CREATE TABLE IF NOT EXISTS coding_goal(
                                value INT NOT NULL,
                                start_date DATE NOT NULL,
                                due_date DATE NOT NULL
                            )";
            connection.Execute(sql);
        }
    }

    public static SQLiteConnection OpenConnection()
    {
        string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");
        var connection = new SQLiteConnection(connectionString);
        connection.Open();

        return connection;
    }

    public static void InsertSession(CodingSession session)
    {
        using (var connection = OpenConnection())
        {
            string sql = @"INSERT INTO coding_sessions (start_date, end_date)
                            VALUES (@StartDateTime, @EndDateTime)";
            var anonymousSession = new
            {
                StartDateTime = session.Start.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDateTime = session.End.ToString("yyyy-MM-dd HH:mm:ss")
            };

            try
            {
                connection.Execute(sql, anonymousSession);
            }
            catch (SQLiteException)
            {
                PrintDbError();
            }
        }
    }

    public static bool PromptSessionOverlap(CodingSession session)
    {
        List<CodingSession> sessions = null;

        using (var connection = OpenConnection())
        {
            string sql = @"SELECT rowid, start_date, end_date FROM coding_sessions
                            WHERE start_date <= @EndDateTime AND end_date >= @StartDateTime AND rowid != @Id";
            var anonymousSession = new
            {
                StartDateTime = session.Start.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDateTime = session.End.ToString("yyyy-MM-dd HH:mm:ss"),
                Id = session.Id
            };

            try
            {
                sessions = connection.Query<CodingSession>(sql, anonymousSession).ToList();
            }
            catch (SQLiteException)
            {
                PrintDbError();
                return false;
            }
        }

        if (sessions.Count == 0)
        {
            return true;
        }

        Console.WriteLine();
        Console.WriteLine(ApplicationTexts.DATASERVICE_OVERLAP_INFO);

        DateUtils.DrawSessionTable(sessions);

        Console.WriteLine();
        var userChoice = AnsiConsole.Prompt(
            new ConfirmationPrompt(ApplicationTexts.DATASERVICE_OVERLAP_PROMPT)
            {
                DefaultValue = false
            }
        );

        if (!userChoice)
        {
            return false;
        }

        userChoice = AnsiConsole.Prompt(
            new ConfirmationPrompt(ApplicationTexts.CONFIRM_AGAIN)
            {
                DefaultValue = false
            }
        );

        if (!userChoice)
        {
            return false;
        }

        using (var connection = OpenConnection())
        {
            foreach (var existingSession in sessions)
            {
                string sql = "DELETE FROM coding_sessions WHERE rowid=@Id";

                try
                {
                    connection.Execute(sql, new { Id = existingSession.Id });
                }
                catch (SQLiteException)
                {
                    PrintDbError();
                    return false;
                }
            }
        }

        return true;
    }

    public static void PrintDbError()
    {
        AnsiConsole.MarkupLine("[red]" + ApplicationTexts.GENERIC_DB_ERROR + "[/]");
        Console.ReadLine();
    }
}