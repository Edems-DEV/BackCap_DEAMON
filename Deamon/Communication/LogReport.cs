using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Communication;
internal class LogReport
{
    public List<Log> Reports { get; set; } = new List<Log>();

    public void AddReport(string message)
    {
        Reports.Add(new Log 
        {
            Message = message,
            Time = DateTime.Now,
            Status = 1
        });
    }

    public void SendReport(string message)
    {
        //poslaní zprávy
    }
}
