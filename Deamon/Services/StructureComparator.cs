using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class StructureComparator
{
    public List<string> GetAllPaths(Folder folder, List<string> paths)
    {
        if(folder == null) 
            return paths;

        foreach (var item in folder.files)
        {
            paths.Add(item.SourcePath);
        }

        foreach (var item in folder.folders)
        {
            paths.Add(item.SourcePath);
            GetAllPaths(item, paths);
        }
        
        return paths;


    }
}
