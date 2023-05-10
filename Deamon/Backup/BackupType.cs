using Deamon.Communication;
using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Backup;
public abstract class BackupType
{
    private protected Config config { get; set; }
    private protected string retencionPath; // opravit retenci ať kontroluje všechny
    private int snapchotNumber = 0;
    private protected Paths paths = new Paths();

    public BackupType(Config config)
    {
        this.config = config;
        this.retencionPath = paths.RoamingPath + @$"\Retencion_{config.Destinations[0].Id}.txt";

        // prvotní generace souborů // to do special třída na cesty // constanty
        foreach (Destination destination in config.Destinations)
        {
            if (!File.Exists(paths.RoamingPath + $@"\Retencion_{destination.Id}.txt"))
            {
                FileStream fileStream = File.Create(paths.RoamingPath + $@"\Retencion_{destination.Id}.txt");
                fileStream.Close();
            }
        }

        if (!File.Exists(paths.SnapchotNumberPath))
        {
            FileStream fileStream = File.Create(paths.SnapchotNumberPath);
            fileStream.Close();

            using (StreamWriter sw = new StreamWriter(paths.SnapchotNumberPath))
            {
                sw.Write(0.ToString());
            }
        }

    }

    public virtual void Backup()
    {
        using (StreamReader rd = new StreamReader(paths.SnapchotNumberPath))
        {
            snapchotNumber = Convert.ToInt32(rd.ReadLine());
        }

        string path = paths.RoamingPath + $"Snapchot_{snapchotNumber % config.Retention}.txt";
        if (CheckSnapshot())
        {
            path = paths.RoamingPath + $"Snapchot_{snapchotNumber % config.Retention}.txt";
            FileStream fs = File.Create(path);
            fs.Close();
        }


        //načtení jsonu ze souboru
        string json = string.Empty;
        using (StreamReader sr = new StreamReader(path))
        {         
            json = sr.ReadToEnd();
        }


        foreach (Destination destiantion in config.Destinations)
        {
            string filepath = Path.Combine(destiantion.DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}");
            Retencion retencion = new Retencion(config.Id, destiantion.Id, config.Retention, config.PackageSize);
            retencion.ReadRetencion();

            foreach (Sources source in config.Sources)
            {
                List<string> different = new List<string>();
                try
                {
                    different = DirectoryRead(source, json);
                }
                catch (Exception)
                {
                    LogReport.AddReport("Vyskytl se problém s čtením souborů");
                }
               

                try
                {
                    DirectoryCreate(filepath, different, source);
                }
                catch (Exception)
                {
                    LogReport.AddReport("Vyskytl se problém s kopírováním souborů");
                }
                
            }


            retencion.WriteRetencion(filepath);
        }

        UpdateSnapchot(SourceToJson(json), path);
    }

    public void DirectoryCreate(string filepath, List<string> different, Sources source)
    {
        different.Sort();

        if (different.Count == 0)
        {
            Directory.CreateDirectory(filepath);
        }
        else
        {
            foreach (var item in different)
            {
                string result = item.Remove(0, source.Path.Length);
                string destpath = filepath + result;

                if (Directory.Exists(item))
                    Directory.CreateDirectory(destpath);
                else
                    File.Copy(item, destpath); // když existují dva stejné fily tak to spadně // to do try catch
            }
        }        
    }

    public List<string> DirectoryRead(Sources source,string json)
    {
        JsonConvertor convert = new JsonConvertor();
        Folder newDirectory = convert.CreateStructrue(new Folder("A", source.Path));

        Folder SnapDirectory = JsonConvert.DeserializeObject<Folder>(json);


        StructureComparator comparator = new StructureComparator();
        List<string> snapPaths = comparator.GetAllPaths(SnapDirectory, new List<string>());
        List<string> newPaths = comparator.GetAllPaths(newDirectory, new List<string>());
        List<string> different = new List<string>();

        foreach (string diffPath in newPaths)
        {
            if (!snapPaths.Contains(diffPath))
            {
                different.Add(diffPath);
            }
        }

        return different;
    }

    public virtual bool CheckSnapshot()
    {
        List<string> paths = new();

        using (StreamReader sr = new StreamReader(retencionPath))
        {
            while (!sr.EndOfStream)
            {
                paths.Add(sr.ReadLine());
            }
        }

        if (paths.Count % config.PackageSize == 0 || paths.Count == 0)
        {
            this.snapchotNumber += 1;
            UpdateSnapchotNumber();
            return true;
        }

        return false;
    }
       
    public abstract void UpdateSnapchot(string json,string path); // každá třída udělá update podle sebe. Full-Žádný, Diff-Update při prvnim, Inc-Sloučí
    // možná bude potřeba dodělat classu snapchot kvůli retenci. Ještě v plánování

    public string SourceToJson(string json)
    {
        // vytvoření jsonu pro každý source. A jejich následná kombinace do jednoho 
        foreach (Sources source in config.Sources)
        {
            JsonCombiner jsonCombiner = new JsonCombiner();
            JsonConvertor jsonConvertor = new JsonConvertor();

            Folder newDirectory = jsonConvertor.CreateStructrue(new Folder($"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}", source.Path));
            string jsonTemp = JsonConvert.SerializeObject(newDirectory, Formatting.Indented);

            json = jsonCombiner.MergeJsons(json, jsonTemp);
        }

        return json;
    }

    private void UpdateSnapchotNumber()
    {
        using (StreamWriter wr = new StreamWriter(paths.SnapchotNumberPath))
        {
            wr.Write(snapchotNumber.ToString());
        }
    }
}
