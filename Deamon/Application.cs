using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Deamon;
public class Application
{
    private readonly HttpClient client = new HttpClient();
    private FileGetter fileGetter = new FileGetter();
    private Paths paths = new();
    private Dictionary<int, System.Timers.Timer> Backuping = new Dictionary<int, System.Timers.Timer>();
    private Job job;
    private Convertor convertor = new();

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056/");
    }

    public async Task SendReports(Log log)
    {
       //TODO - může mít error
       await client.PostAsJsonAsync("api/LogsController",log);
    }

    public async Task GetJobsToFile(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        int? id = fileGetter.GetID();

        if (id == null)
        {
            MachineManager machineManager = new MachineManager();
            MachineDto machine = machineManager.GetLocalMachine();

            var response = await client.PostAsJsonAsync("/api/Machines/register", machine);

            if (!response.IsSuccessStatusCode)
            {
                LogReport.AddReport("Nepovedlo se odeslat informace o stroji na server.");
                return;
            }

            id = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            fileGetter.SaveIdToFile(Convert.ToInt32(id));
        }

        string json;
        try
        {            
            json = await client.GetStringAsync($"/api/Jobs/{id}/daemon");
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
        this.job = jobGetter.GetJobsFromFile(); //getování jobů z filu

        if (job == null)
        {
            Console.WriteLine("Nejsou data. Záloha neprovedena");
            return;
        }

        if (!Backuping.ContainsKey(job.Id_Config))
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = convertor.CronConvertorMilliseconds(job.Config.backup_interval);
            timer.Elapsed += Backup;
            timer.AutoReset = false;
            Backuping.Add(job.Config.Id, timer);
            timer.Start();
        }
    }

    public void Backup(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        Console.WriteLine("Proběhl backup");
        JobManager getJobs = new JobManager();
        BackupType jobtype = getJobs.GetJobTypes(job);

        Backuping[job.Id_Config].Interval = convertor.CronConvertorMilliseconds(job.Config.backup_interval);
        Backuping[job.Id_Config].Start();

        jobtype.Backup();
    }

}
