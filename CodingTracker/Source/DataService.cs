using System.Configuration;
using System.Data.SQLite;
using Dapper;
using vcesario.CodingTracker;

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

            connection.Execute(sql, anonymousSession);
        }
    }
}