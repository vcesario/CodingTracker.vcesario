using System.Configuration;
using System.Data.SQLite;

public static class DataService
{
    private static SQLiteConnection s_Connection;
    public static SQLiteConnection Connection => s_Connection;

    public static void Initialize()
    {
        string connectionString = ConfigurationManager.AppSettings.Get("connectionString");
        s_Connection = new SQLiteConnection(connectionString);
        s_Connection.Open();
    }

    public static void Shutdown()
    {
        s_Connection.Close();
    }
}