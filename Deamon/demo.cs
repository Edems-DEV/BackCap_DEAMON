using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon;
internal class demo
{
}

//using System.Globalization;
//using System.IO.Compression;

//namespace backtest;

//internal class Program
//{
//    static void Main(string[] args)
//    {
//        Backup bu = new IncrementalBackup(@"C:\Users\root\Desktop\Playground\Source", @"C:\Users\root\Desktop\Playground\Target", 2, 2);
//        bu.Run();
//    }
//}

////enum BackupTypes{Full,Inc,Diff}

//public abstract class Backup
//{
//    protected DirectoryInfo sourceInfo;
//    protected DirectoryInfo targetInfo;
//    protected string type; //for backup name
//    public Backup(string sourceDir, string targetDir)
//    {
//        //this.sourceDir = sourceDir; // sourceDir.FullName
//        sourceInfo = new DirectoryInfo(sourceDir);
//        targetInfo = new DirectoryInfo(targetDir);
//    }

//    public abstract void Run();


//    #region Utility (delete?)
//    public string CreateName(string extension = "")//zip //broken
//    {
//        // Creates a unique backup file name with timestamp
//        // Deamon
//        string timestemp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
//        return $"{sourceInfo.Name}_{timestemp}{extension}";
//        //return sourceInfo.Name + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + extension;
//    }
//    #endregion
//}

//public class FullBackup : Backup
//{

//    public FullBackup(string sourceDir, string targetDir) : base(sourceDir, targetDir)
//    {
//        type = "Full";
//    }

//    public override void Run()
//    {
//        FileInfo[] sourceFiles = sourceInfo.GetFiles("*.*", SearchOption.AllDirectories);

//        // Create the target directory if it doesn't exist
//        if (!targetInfo.Exists)
//            targetInfo.Create();

//        // Copy all files from the source directory to the target directory
//        foreach (FileInfo sourceFile in sourceFiles)
//        {
//            string targetFilePath = sourceFile.FullName.Replace(sourceInfo.FullName, targetInfo.FullName);
//            FileInfo targetFile = new FileInfo(targetFilePath);

//            Directory.CreateDirectory(targetFile.Directory.FullName);
//            File.Copy(sourceFile.FullName, targetFile.FullName, true);
//            Console.WriteLine($"Copied file: {sourceFile.FullName} to {targetFile.FullName}"); //testing
//        }
//    }
//}

//public class DifferentialBackup : Backup
//{
//    public DifferentialBackup(string sourceDir, string targetDir) : base(sourceDir, targetDir)
//    {
//        type = "Diff";
//    }

//    public override void Run()
//    {
//        FileInfo[] sourceFiles = sourceInfo.GetFiles("*.*", SearchOption.AllDirectories);
//        FileInfo[] targetFiles = targetInfo.GetFiles("*.*", SearchOption.AllDirectories);

//        foreach (FileInfo sourceFile in sourceFiles)
//        {
//            string targetFilePath = sourceFile.FullName.Replace(sourceInfo.FullName, targetInfo.FullName);
//            FileInfo targetFile = new FileInfo(targetFilePath);

//            if (!targetFile.Exists || targetFile.LastWriteTime < sourceFile.LastWriteTime) //----------------> only difference
//            {
//                Directory.CreateDirectory(targetFile.Directory.FullName);
//                File.Copy(sourceFile.FullName, targetFile.FullName, true);
//                Console.WriteLine($"Copied file: {sourceFile.FullName} to {targetFile.FullName}");
//            }
//        }
//    }
//}

//public class IncrementalBackup : Backup
//{

//    private readonly int maxPackageSize;
//    private readonly int retentionDays;

//    public IncrementalBackup(string sourceDir, string targetDir, int maxPackageSize, int retentionDays) : base(sourceDir, targetDir)
//    {
//        type = "Inc";

//        this.maxPackageSize = maxPackageSize;
//        this.retentionDays = retentionDays;
//    }

//    public override void Run()
//    {
//        // Create the target directory if it doesn't exist
//        if (!targetInfo.Exists)
//            targetInfo.Create();

//        // Get the current date and time for the backup package name
//        DateTime now = DateTime.Now;
//        string backupPackageName = $"backup_{now:yyyyMMdd_HHmmss}";

//        // Create the backup package directory
//        DirectoryInfo backupPackageDirectory = new DirectoryInfo(Path.Combine(targetInfo.FullName, backupPackageName));
//        backupPackageDirectory.Create();

//        // Create a snapshot of the current target directory state
//        DirectoryInfo currentSnapshotDirectory = new DirectoryInfo(Path.Combine(targetInfo.FullName, "current"));
//        if (currentSnapshotDirectory.Exists)
//        {
//            currentSnapshotDirectory.Delete(true);
//        }
//        DirectoryCopy(targetInfo.FullName, currentSnapshotDirectory.FullName);

//        // Copy all new and modified files from the source directory to the backup package directory
//        long packageSize = 0;
//        foreach (FileInfo sourceFile in sourceInfo.GetFiles("*", SearchOption.AllDirectories))
//        {
//            string targetFilePath = sourceFile.FullName.Replace(sourceInfo.FullName, backupPackageDirectory.FullName);
//            FileInfo targetFile = new FileInfo(targetFilePath);

//            if (!targetFile.Exists || targetFile.LastWriteTime < sourceFile.LastWriteTime)
//            {
//                Directory.CreateDirectory(targetFile.Directory.FullName);
//                File.Copy(sourceFile.FullName, targetFile.FullName, true);
//                Console.WriteLine($"Copied file: {sourceFile.FullName} to {targetFile.FullName}");
//                packageSize += targetFile.Length;
//            }

//            // Check if the package size limit has been reached
//            if (packageSize > maxPackageSize)
//            {
//                // Create a new backup package
//                backupPackageName = $"backup_{now:yyyyMMdd_HHmmss}";
//                backupPackageDirectory = new DirectoryInfo(Path.Combine(targetInfo.FullName, backupPackageName));
//                backupPackageDirectory.Create();

//                // Reset the package size counter
//                packageSize = 0;
//            }
//        }

//        // Remove old backup packages
//        DirectoryInfo[] backupDirectories = targetInfo.GetDirectories("backup_*").OrderByDescending(d => d.Name).ToArray();
//        DateTime retentionDate = now.AddDays(-retentionDays);
//        foreach (DirectoryInfo backupDirectory in backupDirectories)
//        {
//            if (backupDirectory.CreationTime < retentionDate)
//            {
//                backupDirectory.Delete(true);
//                Console.WriteLine($"Deleted old backup package: {backupDirectory.Name}");
//            }
//        }
//    }

//    private void DirectoryCopy(string sourceDirectory, string targetDirectory)
//    {
//        DirectoryInfo source = new DirectoryInfo(sourceDirectory);
//        DirectoryInfo target = new DirectoryInfo(targetDirectory);

//        // Recursively copy files and directories
//        foreach (DirectoryInfo sourceSubdirectory in source.GetDirectories())
//        {
//            DirectoryInfo targetSubdirectory = target.CreateSubdirectory(sourceSubdirectory.Name);
//            DirectoryCopy(sourceSubdirectory.FullName, targetSubdirectory.FullName);
//        }
//        foreach (FileInfo sourceFile in source.GetFiles())
//        {
//            string targetFilePath = Path.Combine(target.FullName, sourceFile.Name);
//            sourceFile.CopyTo(targetFilePath);
//        }
//    }
//}


//public class IncrementalBackup2 //idk
//{
//    private int _packageSize; // Maximum size of backup package in bytes
//    private int _retentionDays; // Number of days to retain old backup packages
//    private List<string> _snapshots; // List of snapshot paths for incremental backups

//    public IncrementalBackup2(int packageSize, int retentionDays)
//    {
//        _packageSize = packageSize;
//        _retentionDays = retentionDays;
//        _snapshots = new List<string>();
//    }

//    public void AddSnapshot(string snapshotPath)
//    {
//        _snapshots.Add(snapshotPath);
//    }

//    public void PerformBackup(string backupPath)
//    {
//        // Create new backup package
//        string packagePath = Path.Combine(backupPath, $"backup_{DateTime.Now.ToString("yyyyMMddHHmmss")}");
//        Directory.CreateDirectory(packagePath);

//        // Add files to backup package
//        int packageSize = 0;
//        foreach (string snapshotPath in _snapshots)
//        {
//            string[] files = Directory.GetFiles(snapshotPath, "*", SearchOption.AllDirectories);
//            foreach (string file in files)
//            {
//                FileInfo fileInfo = new FileInfo(file);
//                if (fileInfo.LastWriteTime > DateTime.Now.AddDays(-1)) // Only include files modified within the last day
//                {
//                    string relativePath = file.Replace(snapshotPath, "");
//                    string destinationPath = Path.Combine(packagePath, relativePath);
//                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
//                    File.Copy(file, destinationPath, true);

//                    packageSize += (int)fileInfo.Length;
//                    if (packageSize >= _packageSize) // Maximum package size reached, create new package
//                    {
//                        packagePath = Path.Combine(backupPath, $"backup_{DateTime.Now.ToString("yyyyMMddHHmmss")}");
//                        Directory.CreateDirectory(packagePath);
//                        packageSize = 0;
//                    }
//                }
//            }
//        }

//        // Delete old backup packages
//        string[] backupPackages = Directory.GetDirectories(backupPath, "backup_*");
//        foreach (string backupPackage in backupPackages)
//        {
//            DateTime packageDate = DateTime.ParseExact(backupPackage.Replace(backupPath + "\\backup_", ""), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
//            if (packageDate < DateTime.Now.AddDays(-_retentionDays))
//            {
//                Directory.Delete(backupPackage, true);
//            }
//        }
//    }
//}