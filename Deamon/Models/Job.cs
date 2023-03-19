using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Job
{
    public int Id { get; set; }

    public int Id_Machine { get; set; }

    public int? Id_Group { get; set; }

    public int Id_Config { get; set; }

    public Int16 Status { get; set; }

    public DateTime Time_schedule { get; set; }

    public DateTime? Time_start { get; set; }

    public DateTime? Time_end { get; set; }

    public int? Bytes { get; set; }

    [ForeignKey("Id_Config")]
    public virtual Config Config { get; set; }
}
