﻿using Deamon.Communication;
using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class JsonConvertor
{
    public async Task<Folder> CreateStructrue(Folder folderList)
    {
        if(!Directory.Exists(folderList.SourcePath))
        {
            await LogReport.AddReport("Folder isn't valid");
            return folderList;
        }

        var sourceInfo = new DirectoryInfo(folderList.SourcePath);

        foreach (var sourceSubInfo in sourceInfo.GetFileSystemInfos())
        {
            if (sourceSubInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                folderList.folders.Add(new Folder(sourceSubInfo.Name, Path.Combine(folderList.SourcePath, sourceSubInfo.Name)));
                await CreateStructrue(folderList.folders[folderList.folders.Count - 1]);
            }
            else
            {
                folderList.files.Add(new Record(sourceSubInfo.Name, Path.Combine(folderList.SourcePath, sourceSubInfo.Name)));
            }
        }

        return folderList;
    }
}
