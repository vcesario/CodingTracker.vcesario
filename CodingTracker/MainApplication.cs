namespace vcesario.CodingTracker;

public static class MainApplication
{
    public static void Run()
    {
        Console.WriteLine("Running...");
        Console.WriteLine(DataService.Connection.ToString());
        Console.ReadLine();
    }
}