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

        Folder folder = convert.CreateStructrue(Config.Sources[0].Path, new Folder("A"));

        //Folder A = convert.CreateStructrue(@"C:\Users\Uzivatel\OneDrive\Plocha\A", new Folder("A"));

        string snapPath = Directory.GetCurrentDirectory() + @$"\{Config.Id}_Snapshot.txt";

        if (File.Exists(snapPath)) // prvotní záloha snap neexistuje
        {
            // záloha
            using (StreamWriter writer = new StreamWriter(snapPath))
            {
                Folder snapDirectory = convert.CreateStructrue(Config.Sources[0].Path, new Folder("A")); //zatím jeden source poté bude brát kombinaci jsonu
                string snapJson = JsonConvert.SerializeObject(snapDirectory, Formatting.Indented);
                writer.WriteLine(snapJson);
            }
        }
        else //další záloha. Pokud již existuje snap 
        {
            Folder newDirectory = convert.CreateStructrue(Config.Sources[0].Path, new Folder("A"));
            string newJson = JsonConvert.SerializeObject(newDirectory, Formatting.Indented);

            Folder snapDirectory;
            using (StreamReader reader = new StreamReader(snapPath))
            {
                string snapJson = reader.ReadToEnd();

                JToken parsedJson = JToken.Parse(snapJson);
                snapJson = parsedJson.ToString(Formatting.Indented);

                snapDirectory = JsonConvert.DeserializeObject<Folder>(snapJson);
            }

            if (newJson == JsonConvert.SerializeObject(snapDirectory, Formatting.Indented))
                Console.WriteLine("Same");
            else
            {
                Console.WriteLine("Different");
                //tady bude ukládání
            }
        }

    }
}
