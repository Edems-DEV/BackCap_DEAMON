using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Machine
{
    public int? id { get; set; } = null;
    public string? Name { get; set; } = null;

    public string? Description { get; set; } = null;

    public string? Os { get; set; } = null;

    public string? Ip_Address { get; set; } = null;

    public string? Mac_Address { get; set; } = null;

    public bool Is_Active { get; set; } = false;
}
