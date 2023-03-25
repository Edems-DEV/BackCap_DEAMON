using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Record
{
    public Record(string name)
    {
        this.Name = name;
    }

    public string Name { get; set; }
}
