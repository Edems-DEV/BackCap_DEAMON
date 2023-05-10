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

    public Application()
    {
        client.BaseAddress = new Uri("http://localhost:5056/");
    }

    public async void SendReport(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        foreach (Log item in LogReport.Reports)
        {
            JsonConvert.SerializeObject(item, Formatting.Indented);

            client.PostAsJsonAsync("/api/Logs", item);
        }

        LogReport.Reports.Clear();
    }

    public async void GetJobsToFile(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        int? id = fileGetter.GetID();

        if (id == null)
        {
            MachineDto machine = new MachineDto()
            {
                Name = Environment.MachineName.ToString(),
                Description = "unknown machine",
                Os = Environment.OSVersion.ToString().Substring(0, 20),
                Ip_Address = GetLocalIPAddress(),
                Mac_Address = BitConverter.ToString
                (
                    NetworkInterface.GetAllNetworkInterfaces()
                                    .FirstOrDefault()
                                    .GetPhysicalAddress()
                                    .GetAddressBytes())
            };

            var response = await client.PostAsJsonAsync("/api/Machines/register",machine);

            if (!response.IsSuccessStatusCode)
            {
                LogReport.AddReport("Nepovedlo se odeslat informace o stroji na server.");
                return;
            }

            // dodělat získání id
            id = Convert.ToInt32(await response.Content.ReadAsStringAsync());

            fileGetter.SaveIdToFile(Convert.ToInt32(id));
        }

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



        if (job == null)
            return;

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
                Backuping[job.Config.Id] = convertor.CronConvertorDateTime(job.Config.Backup_interval);
            }
        }
        else
        {
            // první inicializace pro nový config
            DateTime interval = convertor.CronConvertorDateTime(job.Config.Backup_interval); // součet času teď a intervalu převedeného na milisekundy
            Backuping.Add(job.Config.Id, interval);
        }



        //testovací verze, bez získávání dat
        //#region TestJob
        ////test
        //Destination destination1 = new Destination()
        //{
        //    Id = 1,
        //    Id_Config = 1,
        //    DestPath = @"C:\Users\cyril\Desktop\Destination"
        //};

        //Destination destination2 = new Destination()
        //{
        //    Id = 2,
        //    Id_Config = 1,
        //    DestPath = @"C:\Users\cyril\Desktop\Dest2"
        //};

        //Sources source = new Sources()
        //{
        //    Id = 1,
        //    Id_Config = 1,
        //    Path = @"C:\Users\cyril\Desktop\Source"
        //};

        //Sources source2 = new Sources()
        //{
        //    Id = 2,
        //    Id_Config = 1,
        //    Path = @"C:\Users\cyril\Desktop\Source2"
        //};

        //Config config = new Config()
        //{
        //    Id = 1,
        //    Retention = 2,
        //    PackageSize = 3,
        //    Type = 1,
        //    Sources = new List<Sources> { source, source2 },
        //    Destinations = new List<Destination> { destination1, destination2 }
        //};

        //Job jobtest = new Job()
        //{
        //    Id = 1,
        //    Id_Config = 1,
        //    Config = config
        //};
        //#endregion

        //JobManager getJobsTest = new JobManager();
        //BackupType jobtypetest = getJobsTest.GetJobTypes(jobtest);
        //jobtypetest.Backup();

    }
    public static string GetLocalIPAddress()
    {
        foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Only consider Ethernet network interfaces (to exclude virtual interfaces, loopback, etc.)
            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet && netInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                {
                    // Only consider IPv4 addresses
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        return null;
    }

}
