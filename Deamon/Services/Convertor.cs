using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class Convertor
{
    public int CronConvertor(string interval)
    {
        var next = CrontabSchedule.Parse(interval).GetNextOccurrence(DateTime.Now);
        return (int)next.Subtract(DateTime.Now).TotalMilliseconds;
    }
}
