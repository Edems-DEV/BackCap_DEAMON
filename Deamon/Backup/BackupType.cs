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
    private protected string retencionPath = @"C:\Users\Uzivatel\AppData\Roaming\Retencion.txt";

    public BackupType(Config config)
    {
        this.config = config;
    }

    public virtual void Backup()
    {
        //kontrola snapshotu, vyřešit delete
        (bool create, int snapshotCount) = CheckSnapshot();

        string path = string.Empty;
        if(create)
        {
            path = $@"C:\Users\Uzivatel\AppData\Roaming\Snapchot_{snapshotCount}.txt";
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

            foreach (Sources source in config.Sources)
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
                different.Sort();

                if(different.Count == 0)
                {
                    Directory.CreateDirectory(filepath);
                    continue;
                }

                foreach (var item in different)
                {
                    string result = source.Path.Remove(0, source.Path.Length);
                    string destpath = filepath + @"\" + result;

                    if (Directory.Exists(item))
                        Directory.CreateDirectory(destpath);
                    else
                        File.Copy(item, destpath);
                }
            }
        }



        //backup -- záloha těch co nejsou v jsonu -- dodělat


        // vytvoření jsonu pro každý source. A jejich následná kombinace do jednoho -přesunoto do metody
        UpdateSnapchot(SourceToJson(json), path);
    }

    public virtual (bool,int) CheckSnapshot()
    {
        List<string> paths = new();

        using (StreamReader sr = new StreamReader(retencionPath))
        {
            while (!sr.EndOfStream)
            {
                paths.Add(sr.ReadLine());
            }
        }

        if (paths.Count % config.PackageSize == 0)
            return (true, paths.Count / config.PackageSize);

        return (false,0);
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

    
}
