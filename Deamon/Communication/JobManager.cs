using Deamon.Backup;
using Deamon.Models;
using Deamon.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Communication;
public class JobManager
{
    public BackupType GetJobTypes(Job job)
    {
        switch (job.Config.Type)
        {
            case 0:
                {
                    return new FullBackup(job.Config);
                }

            case 1:
                {
                    return new DiferencialBackup(job.Config);
                }

            case 2:
                {
                    return new IncrementalBackup(job.Config);
                }

            default:
                {
                    return new FullBackup(job.Config);
                }
        }
    }
}
