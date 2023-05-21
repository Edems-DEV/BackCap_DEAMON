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
using System.Threading.Tasks;

namespace Deamon;
public class Application
{
    private readonly HttpClient client = new HttpClient();
    private FileGetter fileGetter = new FileGetter();
    private Paths paths = new();
    private Dictionary<int, Timer> Backuping = new Dictionary<int, Timer>();
    private Job job;

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
        Job job = jobGetter.GetJobsFromFile(); //getování jobů z filu
        Convertor convertor = new Convertor();

        if (job == null)
        {
            Console.WriteLine("Nejsou data. Záloha neprovedena");
            return;
        }

        if (!Backuping.ContainsKey(job.Config.Id))
        {
            // první inicializace pro nový config
            Timer CronTimer = new Timer(Backup, null, convertor.CronConvertorMilliseconds(job.Config.backup_interval), 1000);            
            Backuping.Add(job.Config.Id, CronTimer);            
        }
    }

    public void Backup(object state)
    {
        Console.WriteLine("Proběhl backup");
        JobManager getJobs = new JobManager();
        BackupType jobtype = getJobs.GetJobTypes(job);
        jobtype.Backup();
    }

}
