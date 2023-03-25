using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Deamon;

public class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Application application = new Application();
        application.Run(sw.ElapsedMilliseconds);

        //while (true)
        //{
        //    await application.Run(sw.ElapsedMilliseconds);
        //    System.Threading.Thread.Sleep(1000 * 3600);
        //}
    }
}
