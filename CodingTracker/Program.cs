using System.Configuration;
using System.Data.SQLite;
using vcesario.CodingTracker;

string? connectionString = ConfigurationManager.AppSettings.Get("connectionString");
using (var connection = new SQLiteConnection(connectionString))
{
    connection.Open();

    MainApplication.Run();
}
