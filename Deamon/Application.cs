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
    private Paths paths = new();

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056/");
    }

    public async void GetJobsToFile(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        FileGetter fileGetter = new FileGetter();
        int? id = fileGetter.GetID();

        if (id == null)
            return;

        JobManager getJobs = new JobManager();

        string json;
        try
        {
            json = await client.GetStringAsync($"/api/Jobs/Machine/{id}");
        }
        catch (Exception)
        {
            LogReport.AddReport("Nepovedlo se připojit k serveru. Daemon běží v offline režimu");
            return;
        }

        fileGetter.SaveJobsToFile(json); // uloží joby do filu
    }

    public void Run()
    {
        FileGetter jobGetter = new FileGetter();
        Job job = jobGetter.GetJobsFromFile(); //getování jobů z filu
        Dictionary<int, DateTime> Backuping = new Dictionary<int, DateTime>();
        Convertor convertor = new Convertor();



        //reálná verze -- neni otestovaná, nefunguje spojení se serverem
        if (Backuping.ContainsKey(job.Config.Id))
        {
            // kontrola času v konfigu, jestli má zálohovat
            if (Backuping[job.Config.Id] == DateTime.Now)
            {
                JobManager getJobs = new JobManager();
                BackupType jobtype = getJobs.GetJobTypes(job);
                jobtype.Backup();

                //po záloze znova navýšení času
                Backuping[job.Config.Id] = DateTime.Now.AddMilliseconds(convertor.CronConvertor(job.Config.Backup_interval));
            }
        }
        else
        {
            // první inicializace pro nový config
            DateTime interval = DateTime.Now.AddMilliseconds(convertor.CronConvertor(job.Config.Backup_interval)); // součet času teď a intervalu převedeného na milisekundy
            Backuping.Add(job.Config.Id, interval);
        }



        //testovací verze, bez získávání dat
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

        Sources source2 = new Sources()
        {
            Id = 2,
            Id_Config = 1,
            Path = @"C:\Users\cyril\Desktop\Source2"
        };

        Config config = new Config()
        {
            Id = 1,
            Retention = 2,
            PackageSize = 3,
            Type = 1,
            Sources = new List<Sources> { source, source2 },
            Destinations = new List<Destination> { destination1, destination2 }
        };

        Job jobtest = new Job()
        {
            Id = 1,
            Id_Config = 1,
            Config = config
        };
        #endregion

        JobManager getJobsTest = new JobManager();
        BackupType jobtypetest = getJobsTest.GetJobTypes(jobtest);
        jobtypetest.Backup();
    }

}
