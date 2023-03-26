using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public class DiferencialBackup : BackupType
{
    public DiferencialBackup(Config config) : base(config)
    {
    }

    public override void Backup()
    {
        JsonConvertor convert = new JsonConvertor();

        //Folder A = convert.CreateStructrue(@"C:\Users\Uzivatel\OneDrive\Plocha\A", new Folder("A"));


        //string Mergedjson = "";
        //for (int i = 0; i < Config.Sources.Count; i++) //zkombinuje všehcny sources
        //{
        //    Folder tempDirectory = convert.CreateStructrue(new Folder("A", Config.Sources[0].Path));
        //    string tempJson = JsonConvert.SerializeObject(Config.Sources[i], Formatting.Indented);

        //    Mergedjson = Mergedjson + MergeFolders(Mergedjson, tempJson);
        //}

        string snapPath = Directory.GetCurrentDirectory() + @$"\{Config.Id}_Snapshot.txt";

        if (!File.Exists(snapPath)) // prvotní záloha snap neexistuje
        {

            CopyManager copyManager = new CopyManager();
            copyManager.CopyDirectory(Config.Sources[0].Path, Path.Combine(Config.Destinations[0].DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}"));
            // záloha
            using (StreamWriter writer = new StreamWriter(snapPath))
            {
                Folder snapDirectory = convert.CreateStructrue(new Folder("A", Config.Sources[0].Path)); //zatím jeden source poté bude brát kombinaci jsonu
                string snapJson = JsonConvert.SerializeObject(snapDirectory, Formatting.Indented);
                writer.WriteLine(snapJson);
            }
        }
        else //další záloha. Pokud již existuje snap 
        {
            Folder newDirectory = convert.CreateStructrue(new Folder("A", Config.Sources[0].Path));

            string snapJson = string.Empty;
            using (StreamReader reader = new StreamReader(snapPath))
            {
                snapJson = reader.ReadToEnd();

                JToken parsedJson = JToken.Parse(snapJson);
                snapJson = parsedJson.ToString(Formatting.Indented);
            }
            Folder SnapDirectory = JsonConvert.DeserializeObject<Folder>(snapJson);

            StructureComparator comparator = new StructureComparator();
            List<string> snapPaths = comparator.GetAllPaths(SnapDirectory, new List<string>());
            List<string> newPaths = comparator.GetAllPaths(newDirectory, new List<string>());

            List<string> different = new List<string>();

            foreach (string path in newPaths)
            {
                if (!snapPaths.Contains(path))
                {
                    different.Add(path);
                }
            }

            foreach (Destination item in Config.Destinations)
            {
                foreach (string path in different)
                {
                    string result = path.Remove(0, SnapDirectory.SourcePath.Length);
                    string destpath = item.DestPath + @$"\\backup_{DateTime.Now:yyyy_MM_dd_HHmmss}" + result;

                    Console.WriteLine(destpath);

                    if (Directory.Exists(path))
                    {
                        Directory.CreateDirectory(destpath);
                    }
                    else
                    {
                        string[] parts = destpath.Split(@"\");
                        string dirpath = string.Empty;

                        for (int i = 1; i < parts.Length - 1; i++)
                        {
                            dirpath += @"\" + parts[i];
                        }

                        Directory.CreateDirectory(dirpath);

                        File.Copy(path, destpath);
                    }
                }
            }
        }

    }

    public string MergeFolders(string folderJson1, string folderJson2)
    {
        var folder1 = JsonConvert.DeserializeObject<Folder>(folderJson1);
        var folder2 = JsonConvert.DeserializeObject<Folder>(folderJson2);

        var mergedFolder = new Folder(folder1.Name + "-" + folder2.Name, "");

        foreach (var file in folder1.files)
        {
            mergedFolder.files.Add(file);
        }

        foreach (var folder in folder1.folders)
        {
            mergedFolder.folders.Add(folder);
        }

        foreach (var file in folder2.files)
        {
            if (!mergedFolder.files.Contains(file))
            {
                mergedFolder.files.Add(file);
            }
        }

        foreach (var folder in folder2.folders)
        {
            var matchingFolder = mergedFolder.folders.Find(f => f.Name == folder.Name);
            if (matchingFolder == null)
            {
                mergedFolder.folders.Add(folder);
            }
            else
            {
                matchingFolder.SourcePath += ", " + folder.SourcePath;
                MergeFolders(JsonConvert.SerializeObject(matchingFolder), JsonConvert.SerializeObject(folder));
            }
        }

        return JsonConvert.SerializeObject(mergedFolder);
    }
}
