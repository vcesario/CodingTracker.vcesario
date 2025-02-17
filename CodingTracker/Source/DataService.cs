using System.Configuration;
using System.Data.SQLite;
using Dapper;

public static class DataService
{
    public static void Initialize()
    {
        using (var connection = OpenConnection())
        {
            var createCommand = connection.CreateCommand();
            createCommand.CommandText = @"CREATE TABLE IF NOT EXISTS coding_sessions(
                                            start_date_time DATE NOT NULL,
                                            end_date_time DATE NOT NULL
                                        )";
            createCommand.ExecuteNonQuery();
        }
    }

    public static SQLiteConnection OpenConnection()
    {
        string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");
        var connection = new SQLiteConnection(connectionString);
        connection.Open();

        return connection;
    }

    public static void InsertSession(DateTime start, DateTime end)
    {
        using (var connection = DataService.OpenConnection())
        {
            string insertStatement = @"INSERT INTO coding_sessions (start_date_time, end_date_time)
                                            VALUES (@StartDateTime, @EndDateTime)";
            var anonymousSession = new {
                StartDateTime = start.ToString("yyyy-MM-dd hh:mm:ss"),
                EndDateTime = end.ToString("yyyy-MM-dd hh:mm:ss")
            };

            connection.Execute(insertStatement, anonymousSession);
        }
    }
}