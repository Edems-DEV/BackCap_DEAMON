using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
internal class Differencial : BackupType
{
    public Differencial(Config config) : base(config) { }

    public override void UpdateSnapchot(string json, string path)
    {
        if (new FileInfo(path).Length == 0)
        {
            using (StreamWriter sr = new StreamWriter(path))
            {
                sr.Write(json);
            }
        }
        
    }
}
