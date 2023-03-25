using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using Deamon.Services;
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

    public async Task Run(long time)
    {
        //zaokrouhlení kvůli nepřesnostem
        time = Convert.ToInt64(Math.Floor((double)time));

        GetAddresses addresses = new GetAddresses();
        List<string> ips = addresses.GetIpAddresses();
        JobManager getJobs = new JobManager();
        //List<Job> jobs = await getJobs.GetJobs(ips, client);

        #region TestJob
        //test
        Destination destination1 = new Destination()
        {
            Id = 1,
            Id_Config = 1,
            DestPath = @"C:\Users\cyril\Desktop\Destination"
        };

        Destination destination2 = new Destination()
        {
            Id = 2,
            Id_Config = 1,
            DestPath = @"C:\Users\cyril\Desktop\Dest2"
        };

        Sources source = new Sources()
        {
            Id = 1,
            Id_Config = 1,
            Path = @"C:\Users\cyril\Desktop\Source"
        };

        Config config = new Config()
        {
            Id = 1,
            Retention = 2,
            PackageSize = 1,
            Type = 0,
            Sources = new List<Sources> { source },
            Destinations = new List<Destination> { destination1, destination2 }
        };

        Job jobtest = new Job()
        {
            Id = 1,
            Id_Config = 1,
            Config = config
        };
        #endregion

        List<Job> jobs = new List<Job>();
        jobs.Add(jobtest);


        BackupType jobtype;
        foreach (Job job in jobs)
        {
            jobtype = getJobs.GetJobTypes(job);
            Convertor convertor = new Convertor();
            int interval = convertor.CronConvertor(job.Config.Backup_interval);

            //if (time % interval == 0)
            jobtype.Backup();
        }

        //jakmile event se zálohováním skončí zavolá to další event který pošle report

        //foreach (Job job in jobs) // posílání dat zpátky, bude chtít úpravu
        //{
        //    job.Status = 1;
        //    job.Time_start = DateTime.Now;
        //    job.Time_end = DateTime.Now;
        //    await client.PutAsJsonAsync("/api/Jobs/1/StatusTime", job); // nespojí se, netušim proč, ale ani nespadne
        //}
    }

}
