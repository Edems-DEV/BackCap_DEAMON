using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public abstract class BackupType
{
    public List<DirectoryInfo> SourceList = new List<DirectoryInfo>();
    private protected Config Config;

    protected BackupType(Config config)
    {
        this.Config = config;

        foreach (Sources item in config.Sources)
        {
            DirectoryInfo Dir = new DirectoryInfo(item.Path);
            SourceList.Add(Dir);
        }
    }
    public abstract void Backup();
}
