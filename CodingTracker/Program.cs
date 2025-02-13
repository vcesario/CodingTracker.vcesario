using System.Configuration;
using System.Collections.Specialized;

string value = ConfigurationManager.AppSettings.Get("DataPath");
Console.WriteLine($"MySetting: {value}");

Console.ReadLine();