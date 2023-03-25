using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class CopyManager
{
    public void CopyDirectory(string sourceDir, string destDir)
    {
        var sourceInfo = new DirectoryInfo(sourceDir);
        var destInfo = new DirectoryInfo(destDir);

        foreach (var sourceSubInfo in sourceInfo.GetFileSystemInfos())
        {
            var destSubPath = Path.Combine(destInfo.FullName, sourceSubInfo.Name);

            if (sourceSubInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                // It's a directory, so create it if it doesn't exist
                Directory.CreateDirectory(destSubPath);
                // Now copy the contents recursively
                CopyDirectory(sourceSubInfo.FullName, destSubPath);
            }
            else
            {
                // It's a file, so copy it
                File.Copy(sourceSubInfo.FullName, destSubPath, true);
            }
        }
    }
}
