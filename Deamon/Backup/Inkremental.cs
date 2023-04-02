using Deamon.Models;
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
        throw new NotImplementedException();
    }
}
