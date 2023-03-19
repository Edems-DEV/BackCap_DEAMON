using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Config
{
    public int Id { get; set; }

    public Int16 Type { get; set; }

    public int Retention { get; set; }

    public int PackageSize { get; set; }

    public bool IsCompressed { get; set; }

    public string? Backup_interval { get; set; }

    public DateTime? Interval_end { get; set; }

    public virtual List<Sources> Sources { get; set; }

    public virtual List<Destination> Destinations { get; set; }
}
