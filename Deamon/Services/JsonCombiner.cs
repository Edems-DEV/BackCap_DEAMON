using Deamon.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class JsonCombiner
{

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

        return JsonConvert.SerializeObject(mergedFolder, Formatting.Indented);
    }

    public string MergeJsons(string json1, string json2)
    {
        if (json1 == string.Empty)
            return json2;

        if (json2 == string.Empty)
            return json1;

        JObject obj1 = JObject.Parse(json1);
        JObject obj2 = JObject.Parse(json2);

        obj1.Merge(obj2, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        });

        return obj1.ToString();
    }
}
