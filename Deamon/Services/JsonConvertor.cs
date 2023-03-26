using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class JsonConvertor
{
    public Folder CreateStructrue(string sourceDir, Folder folderList)
    {
        var sourceInfo = new DirectoryInfo(sourceDir);

        foreach (var sourceSubInfo in sourceInfo.GetFileSystemInfos())
        {
            if (sourceSubInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                folderList.folders.Add(new Folder(sourceSubInfo.Name));
                CreateStructrue(sourceSubInfo.FullName, folderList.folders[folderList.folders.Count - 1]);
            }
            else
            {
                folderList.files.Add(new Record(sourceSubInfo.Name));
            }
        }

        return folderList;
    }
}
