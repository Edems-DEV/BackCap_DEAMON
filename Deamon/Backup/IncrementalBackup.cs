using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public class IncrementalBackup : BackupType
{
    public IncrementalBackup(Config config) : base(config)
    {
    }

    public override void Backup()
    {
        throw new NotImplementedException();
    }
}
