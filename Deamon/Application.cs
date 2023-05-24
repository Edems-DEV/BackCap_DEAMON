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
    public Job Job { get; set; }
    private Convertor convertor = new();

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056/");
    }

    public async Task SendReports(Log log)
    {
       //TODO - může mít error
       await client.PostAsJsonAsync("/api/Logs/Add", log);
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
        this.Job = jobGetter.GetJobsFromFile(); //getování jobů z filu

        if (Job == null)
        {
            Console.WriteLine("Nejsou data. Záloha neprovedena");
            return;
        }

        Backuping.Keys.Where(x => x != Job.Id_Config).ToList().ForEach(x => Backuping.Remove(x));

        if (!Backuping.ContainsKey(Job.Id_Config))
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = convertor.CronConvertorMilliseconds(Job.Config.backup_interval);
            timer.Elapsed += Backup;
            timer.AutoReset = false;
            Backuping.Add(Job.Config.Id, timer);
            timer.Start();
        }
    }

    public void Backup(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        JobManager getJobs = new JobManager();
        BackupType jobtype = getJobs.GetJobTypes(Job);

        Backuping[Job.Id_Config].Interval = convertor.CronConvertorMilliseconds(Job.Config.backup_interval);
        Backuping[Job.Id_Config].Start();

        jobtype.Backup();
    }

}
