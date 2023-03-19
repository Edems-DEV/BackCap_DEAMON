using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Deamon;
public class Application
{
    private readonly HttpClient client = new HttpClient();

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056");
    }

    public async Task Run()
    { 
        while (true)
        {
            GetAddresses addresses = new GetAddresses();
            List<string> ips = addresses.GetIpAddresses();
            JobManager getJobs = new JobManager();
            List<Job> jobs = await getJobs.GetJobs(ips, client);


            JobTypes jobtype;
            foreach (Job job in jobs)
            {
                jobtype = getJobs.GetJobTypes(job);
                string message = jobtype.Start(job.Config);
                //pošle report
            }


            foreach (Job job in jobs)
            {
                job.Status = 1;
                job.Time_start = DateTime.Now;
                job.Time_end = DateTime.Now;
                await client.PutAsJsonAsync("/api/Jobs/1/StatusTime", job); // nespojí se netušim proč, ale ani nespadne
            }

            //Console.WriteLine("success");

            System.Threading.Thread.Sleep(1000 * 3600);
        }
    }
}
