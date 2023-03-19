using Deamon.Communication;
using Deamon.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Deamon;

public class Program
{
    static async Task Main(string[] args)
    {
        Application application = new Application();
        application.Run();
    }
}
