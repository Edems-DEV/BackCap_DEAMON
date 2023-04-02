using Deamon.Models;
using Deamon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
internal class Inkremental : BackupType
{
    public Inkremental(Config config) : base(config) { }


    public override void UpdateSnapchot(string json, string path)
    {
        string snapJson = string.Empty;
        using (StreamReader sr = new StreamReader(path))
        {
            snapJson = sr.ReadToEnd();
        }

        JsonCombiner jsonCombiner = new JsonCombiner();
        json = jsonCombiner.MergeJsons(json, snapJson);

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write(json);
        }
    }
}
