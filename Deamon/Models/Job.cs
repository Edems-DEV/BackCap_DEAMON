using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Job
{
    // How?
    public Int16 Type { get; set; }

    public int Retention { get; set; }

    public int PackageSize { get; set; }

    public bool IsCompressed { get; set; }

    // Where?
    public List<string> Sources { get; set; }

    // What?
    public List<string> Destinations { get; set; }
}
