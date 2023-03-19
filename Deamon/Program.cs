using Deamon.Backup;
using Deamon.Communication;
using System.Net.Http.Json;

namespace Deamon;

internal class Program
{
    static async void Main(string[] args)
    {
        ////timer

        //HttpClient client = new HttpClient();
        //client.BaseAddress = new Uri("http://localhost:5035");

        ////User user = await client.GetFromJsonAsync<User>("/api/users/1");
        ////Console.WriteLine(user.Name);

        GetJob getJob = new GetJob();
        BackupProcess backupProcess = new BackupProcess();
        LogReport report = new LogReport();

        getJob.getJob();
        backupProcess.Backup();
        report.SendReport();
    }
}
