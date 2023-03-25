using Deamon.Models;
using Deamon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public class FullBackup : BackupType
{
    public FullBackup(Config config) : base(config) {}

    public override void Backup()
    {
        string destinationPath = "";

        foreach (Destination destination in Config.Destinations)
        {
            DirectoryInfo destItem = new DirectoryInfo(destination.DestPath);
            Retencion retencion = new(Config.Id, destination.Id, 2, 5);
            retencion.FullReadRetancion();

            foreach (DirectoryInfo sourcePath in SourceList)
            {
                destinationPath = Path.Combine(destItem.ToString(), $"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}" /* zde může být jakýkoli název (př: backup_24.1.2004)*/);

                if (sourcePath.Attributes.HasFlag(FileAttributes.Directory))
                {
                    // It's a directory, so create it if it doesn't exist
                    Directory.CreateDirectory(destinationPath);
                    // Now copy the contents recursively
                    CopyManager copyManager = new CopyManager();
                    copyManager.CopyDirectory(sourcePath.ToString(), destinationPath);
                }
                else
                {
                    // It's a file, so copy it
                    File.Copy(sourcePath.ToString(), destinationPath, true);
                }
            }
            retencion.FullWriteRetancion(destinationPath);
        }
    }
}
