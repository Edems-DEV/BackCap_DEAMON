using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
internal class Machine
{
    public string Name { get; set; } //windows name of Pc
    public string Os { get; set; } //del?
    public string Ip { get; set; } // server will see it (should i still send it?)
}

// Only on init
