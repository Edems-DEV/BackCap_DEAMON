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
