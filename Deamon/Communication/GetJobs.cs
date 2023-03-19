using Deamon.Backup;
using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Communication;
public class GetJobs
{
    public async Task<List<Job>> GetJobs(List<string> ips, HttpClient client)
    {
        List<Job> jobs = new();

        foreach (string ip in ips)
        {
            jobs.Add(await client.GetFromJsonAsync<Job>("/api/Jobs/154.251.15.1/Daemon"));  // tady je ip adresa statická pro testování
        }

        return jobs;
    }

    public JobTypes GetJobTypes(Job job)
    {
        switch (job.Config.Type)
        {
            case 1:
                {
                    return new FullBackup();
                }

            case 2:
                {
                    return new DiferencialBackup();
                }

            case 3:
                {
                    return new IncrementalBackup();
                }

            default:
                {
                    return new FullBackup();
                }
        }
    }
}
