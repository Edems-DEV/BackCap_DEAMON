using Deamon.Models;
using System.Runtime.CompilerServices;

namespace Deamon.Communication;
public static class LogReport
{
    public static Application Application { get; set; }

    public static async Task AddReport(string message)
    {
        Console.WriteLine(message);

        if(Application.Job == null)
            return;

        await Application.SendReports(new Log
        {
            Id_Job = Application.Job.Id,
            Message = message,
            Time = DateTime.Now,
        }); 
    }
}
