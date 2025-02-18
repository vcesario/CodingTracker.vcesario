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
            string createStatement = @"CREATE TABLE IF NOT EXISTS coding_sessions(
                                            start_date_time DATE NOT NULL,
                                            end_date_time DATE NOT NULL
                                        )";
            connection.Execute(createStatement);
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
        using (var connection = DataService.OpenConnection())
        {
            string insertStatement = @"INSERT INTO coding_sessions (start_date_time, end_date_time)
                                        VALUES (@StartDateTime, @EndDateTime)";
            var anonymousSession = new
            {
                StartDateTime = session.Start.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDateTime = session.End.ToString("yyyy-MM-dd HH:mm:ss")
            };

            connection.Execute(insertStatement, anonymousSession);
        }
    }
}