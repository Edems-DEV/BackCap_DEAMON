using Deamon.Communication;
using Deamon.Models;
using System.Net.Http.Json;

namespace Deamon;

internal class Program
{
    static async Task Main(string[] args)
    {
        ////timer

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5056");

        Job job = await client.GetFromJsonAsync<Job>("/api/Jobs/154.251.15.1/Daemon");
        Console.WriteLine(job.Id);
        Console.WriteLine(job.Config.Id);
        Console.WriteLine(job.Config.Sources[0].Id);
        Console.WriteLine(job.Config.Destinations[0].Id);
        

        GetJob getJob = new GetJob();
        LogReport report = new LogReport();

        getJob.getJob();
        report.SendReport();
    }
}
