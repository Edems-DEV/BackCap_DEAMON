using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class StructureComparator
{
    public List<string> GetDifferentPaths(Folder oldFolder, Folder newFolder)
    {
        List<string> paths = new List<string>();

        foreach (var oldfolder in oldFolder.folders)
        {
            foreach (var newfolder in newFolder.folders)
            {
                if (oldfolder.SourcePath != newfolder.SourcePath)
                {
                    paths.Add(newfolder.SourcePath);
                }
            }
        }
    }
}
