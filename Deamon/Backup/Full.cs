using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
internal class Full : BackupType
{
    public Full(Config config) : base(config) {}


    public override void UpdateSnapchot(string json, string path)
    {
        return;
    }
}
