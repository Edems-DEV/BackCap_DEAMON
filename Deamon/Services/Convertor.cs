﻿using NCrontab;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class Convertor
{
    public DateTime CronConvertorDateTime(string interval)
    {
        return CrontabSchedule.Parse(interval).GetNextOccurrence(DateTime.Now);
    }

    public long CronConvertorMilliseconds(string interval)
    {
        DateTime now = DateTime.Now;
        DateTime nextOccurrence = CrontabSchedule.Parse(interval).GetNextOccurrence(now);
        TimeSpan timeUntilNextOccurrence = nextOccurrence - now;
        long milliseconds = (long)timeUntilNextOccurrence.TotalMilliseconds;

        if (milliseconds == 0)
            return 1000;

        return milliseconds;
    }

    public long CronLengthMilliseconds(string interval) 
    {
        CrontabSchedule schedule = CrontabSchedule.Parse(interval);
        DateTime now = DateTime.Now;
        DateTime nextOccurrence = schedule.GetNextOccurrence(now);
        TimeSpan timeUntilNextOccurrence = nextOccurrence - now;
        long milliseconds = (long)timeUntilNextOccurrence.TotalMilliseconds;

        if (milliseconds == 0)
            return 1000;

        return milliseconds;
    }
}
