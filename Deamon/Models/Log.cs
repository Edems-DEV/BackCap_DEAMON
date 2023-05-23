using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Log
{
    public int Id { get; set; }
    public int Id_Job { get; set; }

    public string Message { get; set; }

    public DateTime Time { get; set; }
}
