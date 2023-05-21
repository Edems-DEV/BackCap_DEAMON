using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Communication;
public static class LogReport
{
    private static List<Log> reports = new List<Log>();

    public static void AddReport(string message)
    {
        reports.Add(new Log
        {
            Message = message,
            Time = DateTime.Now,
            Status = 1
        });
    }

    //public static List<Log> GetReports()
    //{
    //    return reports;
    //}
}
