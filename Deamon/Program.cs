using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Deamon;

public class Program
{
    static async Task Main(string[] args)
    {
        Application application = new Application();
        application.GetJobsToFile(null, null); // prvotní get dat

        System.Timers.Timer timer = new System.Timers.Timer();
        timer.Interval = 1000 * 60; //minuta
        //každou minutu se zavolá event, který stáhne data ze serveru a uloží je do filu
        //pokud by nebylo připojení/nějaký error. Tak se metoda pouze returne a do filu nic neuloží
        timer.Elapsed += application.GetJobsToFile;
        timer.AutoReset = true;
        timer.Start();

        System.Timers.Timer reportTimer = new System.Timers.Timer();
        timer.Interval = 1000 * 3600; // jednou za hodinu pošle na server report
        timer.Elapsed += application.SendReport;
        timer.AutoReset = true;
        timer.Start();


        while (true)
        {
            application.Run();
            await Task.Delay(1000 * 10); //pauze 10 vteřin
        }
    }
}
