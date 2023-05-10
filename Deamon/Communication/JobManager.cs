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
                    job.Config.PackageSize = 1;
                    return new Full(job.Config);
                }

            case 1:
                {
                    return new Differencial(job.Config);
                }

            case 2:
                {
                    return new Inkremental(job.Config);
                }

            default:
                {
                    LogReport.AddReport("Nevhodný typ zálohování");
                    throw new Exception("Nevhodný typ zálohování");
                }
        }
    
}
