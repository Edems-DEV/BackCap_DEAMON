using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class Folder
{
    public Folder(string name, string sourcePath)
    {
        this.Name = name;
        this.SourcePath = sourcePath;
    }

    public string Name { get; set; }
    public string SourcePath { get; set; }  

    public List<Record> files = new List<Record>();
    public List<Folder> folders = new List<Folder>();

}
