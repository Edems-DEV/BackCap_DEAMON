using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class JsonConvertor
{
    public Folder CreateStructrue(Folder folderList)
    {
        var sourceInfo = new DirectoryInfo(folderList.SourcePath);

        foreach (var sourceSubInfo in sourceInfo.GetFileSystemInfos())
        {
            if (sourceSubInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                folderList.folders.Add(new Folder(sourceSubInfo.Name, Path.Combine(folderList.SourcePath, sourceSubInfo.Name)));
                CreateStructrue(folderList.folders[folderList.folders.Count - 1]);
            }
            else
            {
                folderList.files.Add(new Record(sourceSubInfo.Name, Path.Combine(folderList.SourcePath, sourceSubInfo.Name)));
            }
        }

        return folderList;
    }
}
