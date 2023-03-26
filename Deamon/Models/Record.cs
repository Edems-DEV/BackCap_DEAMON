using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Record
{
    public Record(string name, string sourcePath)
    {
        this.Name = name;
        this.SourcePath = sourcePath;
    }

    public string SourcePath { get; set; }
    public string Name { get; set; }
}
