using Deamon.Models;

namespace Deamon.Communication;
public static class LogReport
{
    public static Application Application { get; set; }

    public async static void AddReport(string message)
    {
        Console.WriteLine(message);

        await Application.SendReports(new Log
        {
            Message = message,
            Time = DateTime.Now,
            Status = 1
        });
    }
}
