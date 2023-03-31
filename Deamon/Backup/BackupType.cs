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

    public BackupType(Config config)
    {
        this.config = config;
    }

    public virtual void Backup()
    {
        string path = string.Empty;

        // kontrola existence jsonu
        if (this.CheckSnapchot(path))
            path = this.CreateSnapchot();


        // načtení jsonu ze souboru
        string json;
        using (StreamReader sr = new StreamReader(path))
        {
            json = sr.ReadToEnd();
        }


        //backup -- záloha těch co nejsou v jsonu -- dodělat


        // vytvoření jsonu pro každý source. A jejich následná kombinace do jednoho
        foreach (Sources source in config.Sources)
        {
            JsonCombiner jsonCombiner = new JsonCombiner();
            JsonConvertor jsonConvertor = new JsonConvertor();

            Folder newDirectory = jsonConvertor.CreateStructrue(new Folder($"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}", source.Path));
            string jsonTemp = JsonConvert.SerializeObject(newDirectory, Formatting.Indented);

            json = jsonCombiner.MergeJsons(json, jsonTemp);
        }

    }

    public string CreateSnapchot()
    {
        return ""; // rozdělané
    }

    public bool CheckSnapchot(string path)
    {
        return false; // rozdělané
    }

    public abstract void UpdateSnapchot(); // každá třída udělá update podle sebe. Full-Žádný, Diff-Update při prvnim, Inc-Sloučí
    // možná bude potřeba dodělat classu snapchot kvůli retenci. Ještě v plánování
}
