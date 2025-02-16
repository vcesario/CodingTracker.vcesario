using System.Configuration;
using System.Data.SQLite;

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
}