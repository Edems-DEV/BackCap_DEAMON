using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Deamon.Backup;
public class IncrementalBackup : BackupType
{
    public IncrementalBackup(Config config) : base(config)
    {
    }

    public override void Backup()
    {
        JsonConvertor convert = new JsonConvertor();

        string snapPath = Directory.GetCurrentDirectory() + @$"\{Config.Id}_Snapshot.txt";

        if (!File.Exists(snapPath)) // prvotní záloha snap neexistuje
        {
            CopyManager copyManager = new CopyManager();

            foreach (Destination destination in Config.Destinations)
            {
                string filepath = Path.Combine(destination.DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}");
                Retencion retencion = new Retencion(Config.Id, destination.Id, Config.Retention, Config.PackageSize);
                retencion.ReadRetencion();

                foreach (Sources source in Config.Sources)
                {
                    copyManager.CopyDirectory(source.Path, filepath);
                }
                retencion.WriteRetencion(filepath);
            }

            // vytvoření snapchotu
            using (StreamWriter writer = new StreamWriter(snapPath))
            {
                List<string> jsons = new List<string>();

                foreach (var item in Config.Sources)
                {
                    Folder snapDirectory = convert.CreateStructrue(new Folder("A", item.Path)); //zatím jeden source poté bude brát kombinaci jsonu
                    string snapJson = JsonConvert.SerializeObject(snapDirectory, Formatting.Indented);
                    jsons.Add(snapJson);
                }

                if (jsons.Count == 1)
                    writer.WriteLine(jsons[0]);
                else
                {
                    JsonCombiner combiner = new JsonCombiner();
                    string combined = jsons[0];
                    for (int i = 1; i < jsons.Count; i++)
                    {
                        combined = combiner.MergeFolders(combined, jsons[i]);
                    }
                    writer.WriteLine(combined);
                }

            }
        }
        else //další záloha. Pokud již existuje snapchot
        {
            Folder newDirectory;
            string newJson = string.Empty;
            Folder SnapDirectory;
            string snapJson = string.Empty;

            string backupPath = string.Empty;

            foreach (Destination destination in Config.Destinations)
            {
                string filepath = Path.Combine(destination.DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}");
                Retencion retencion = new Retencion(Config.Id, destination.Id, Config.Retention, Config.PackageSize);
                retencion.ReadRetencion();
                                
                foreach (Sources source in Config.Sources)
                {
                    newDirectory = convert.CreateStructrue(new Folder("A", source.Path));

                    snapJson = string.Empty;
                    using (StreamReader reader = new StreamReader(snapPath))
                    {
                        snapJson = reader.ReadToEnd();

                        JToken parsedJson = JToken.Parse(snapJson);
                        snapJson = parsedJson.ToString(Formatting.Indented);
                    }
                    SnapDirectory = JsonConvert.DeserializeObject<Folder>(snapJson);

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

                    if (different.Count == 0 && !Directory.Exists(filepath)) // pokud nedlošlo k změně
                    {
                        Directory.CreateDirectory(filepath);
                        retencion.WriteRetencion(filepath);
                        continue;
                    }

                    foreach (string path in different)
                    {
                        string result = path.Remove(0, source.Path.Length);
                        string destpath = destination.DestPath + @$"\\backup_{DateTime.Now:yyyy_MM_dd_HHmmss}" + result;

                        if (Directory.Exists(path)) //kontrola zda je to složka
                        {
                            Directory.CreateDirectory(destpath); //tvoření složky
                        }
                        else // kopírování filu a tvoření složek po cestě
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

                    retencion.WriteRetencion(filepath);
                }

                backupPath = retencion.GetLastPath();
            }            

            JsonConvertor jsonConvertor = new JsonConvertor();
            Folder CurrentFulder =jsonConvertor.CreateStructrue(new Folder("A",backupPath));

            string currentJson = JsonConvert.SerializeObject(CurrentFulder, Formatting.Indented);

            string newSnapJson = string.Empty;
            JsonCombiner combiner = new JsonCombiner();
            newSnapJson = combiner.MergeFolders(currentJson, snapJson);

            using (StreamWriter writer = new StreamWriter(snapPath))
            {
                writer.WriteLine(newSnapJson);
            }
        }
    }
}
