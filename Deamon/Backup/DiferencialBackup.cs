using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public class DiferencialBackup : BackupType
{
    public DiferencialBackup(Config config) : base(config)
    {
    }

    public override void Backup()
    {
        JsonConvertor convert = new JsonConvertor();

        Folder A = convert.CreateJson(@"C:\Users\Uzivatel\OneDrive\Plocha\A", new Folder("A"));

        string json = JsonConvert.SerializeObject(A, Formatting.Indented);

        Console.WriteLine(json);
    }
}
