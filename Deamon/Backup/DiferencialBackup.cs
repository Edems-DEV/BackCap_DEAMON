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
                Folder snapDirectory = convert.CreateStructrue(new Folder("A", Config.Sources[0].Path)); //zatím jeden source poté bude brát kombinaci jsonu
                string snapJson = JsonConvert.SerializeObject(snapDirectory, Formatting.Indented);
                writer.WriteLine(snapJson);
            }
        }
        else //další záloha. Pokud již existuje snapchot
        {
            foreach (Destination destination in Config.Destinations)
            {
                Retencion retencion = new Retencion(Config.Id, destination.Id, Config.Retention, Config.PackageSize);
                retencion.ReadRetencion();

                foreach (Sources source in Config.Sources)
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

                    if (different.Count == 0) // pokud nedlošlo k změně
                    {
                        string filepath = Path.Combine(destination.DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}");
                        Directory.CreateDirectory(filepath);
                        retencion.WriteRetencion(filepath);
                        return;
                    }

                    foreach (string path in different)
                    {
                        string result = path.Remove(0, SnapDirectory.SourcePath.Length);
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
                }
            }

        }

    }
}
