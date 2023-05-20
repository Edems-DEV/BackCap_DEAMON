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
    private Dictionary<int, DateTime> Backuping = new Dictionary<int, DateTime>();

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056/");
    }

    public async Task SendReports(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        await client.PostAsJsonAsync("api/LogsController", LogReport.GetReports);
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
            Console.WriteLine("Nejsou data. Záloha neprovedla");
            return;
        }

        //reálná verze -- neni otestovaná, nefunguje spojení se serverem
        if (Backuping.ContainsKey(job.Config.Id))
        {
            // kontrola času v konfigu, jestli má zálohovat
            if (DateTime.Now - Backuping[job.Config.Id] > TimeSpan.FromSeconds(2))
            {
                JobManager getJobs = new JobManager();
                BackupType jobtype = getJobs.GetJobTypes(job);
                jobtype.Backup();

                //po záloze znova navýšení času
                Backuping[job.Config.Id] = convertor.CronConvertorDateTime(job.Config.Backup_interval);
            }
        }
        else
        {
            // první inicializace pro nový config
            DateTime interval = convertor.CronConvertorDateTime(job.Config.Backup_interval); // součet času teď a intervalu převedeného na milisekundy
            Backuping.Add(job.Config.Id, interval);
        }
    }

}
