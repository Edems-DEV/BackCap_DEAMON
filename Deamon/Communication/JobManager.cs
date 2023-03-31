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
    public BackupTypeTemp GetJobTypes(Job job)
    {
        switch (job.Config.Type)
        {
            case 0:
                {
                    return new FullBackupTemp(job.Config);
                }

            case 1:
                {
                    return new DiferencialBackupTemp(job.Config);
                }

            case 2:
                {
                    return new IncrementalBackupTemp(job.Config);
                }

            default:
                {
                    return new FullBackupTemp(job.Config);
                }
        }
    }
}
