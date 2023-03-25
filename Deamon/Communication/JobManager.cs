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
public class JobManager
{
    public async Task<List<Job>> GetJobs(List<string> ips, HttpClient client)
    {
        List<Job> jobs = new();

        foreach (string ip in ips)
        {
            jobs.Add(await client.GetFromJsonAsync<Job>("/api/Jobs/7/Machine"));  // tady je ip adresa statická pro testování
        }

        return jobs;
    }

    public BackupType GetJobTypes(Job job)
    {
        switch (job.Config.Type)
        {
            case 1:
                {
                    return new FullBackup(job.Config);
                }

            case 2:
                {
                    return new DiferencialBackup(job.Config);
                }

            case 3:
                {
                    return new IncrementalBackup(job.Config);
                }

            default:
                {
                    return new FullBackup(job.Config);
                }
        }
    }
}
