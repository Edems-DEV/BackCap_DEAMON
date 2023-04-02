using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Communication;
public static class LogReport
{
    public static List<Log> Reports { get; set; } = new List<Log>();

    public static void AddReport(string message)
    {
        Reports.Add(new Log 
        {
            Message = message,
            Time = DateTime.Now,
            Status = 1
        });
    }

    public static void SendReport(string message)
    {
        //poslaní zprávy
    }
}
