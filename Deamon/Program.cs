using Deamon.Communication;
using Deamon.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Deamon;

public class Program
{
    static async Task Main(string[] args)
    {
        ////timer

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5056");

        GetAddresses addresses = new GetAddresses();
        List<string> ips = addresses.GetIpAddresses();


        List<Job> jobs = new();   
        foreach (string ip in ips)
        {
            jobs.Add(await client.GetFromJsonAsync<Job>("/api/Jobs/154.251.15.1/Daemon"));  // tady je ip adresa statická pro testování
        } 


        LogReport report = new LogReport();

        report.SendReport();
    }
}
