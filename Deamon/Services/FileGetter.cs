using Deamon.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class FileGetter
{
    private string jobFilePath;

    public FileGetter()
    {
        string pathPart = Directory.GetCurrentDirectory();
        this.jobFilePath = Path.Combine(pathPart, "Jobs.txt");

        if (!File.Exists(this.jobFilePath))
        {
            FileStream filestream = File.Create(this.jobFilePath);
            filestream.Close();
        }
        
    }

    public int? GetID()
    {
        string pathPart = Directory.GetCurrentDirectory();
        string path = Path.Combine(pathPart, "ID.txt");

        if (!File.Exists(path))
        { 
            //vyvolej registraci přidam až s webem kde půjde potvrdit
            return null;
        }
        else
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int ID;
                try
                {
                    ID = Convert.ToInt32(sr.ReadLine());
                }
                catch (Exception)
                {
                    File.Delete(path);
                    return null;
                }
                return ID;
            }
        }
    }

    public void SaveJobsToFile(string json)
    {
        using(StreamWriter sw = new StreamWriter(jobFilePath))
        {
            sw.WriteLine(json);
        }
    }

    public List<Job> GetJobsFromFile()
    {
        List<Job> jobs = new();

        using(StreamReader sr = new StreamReader(this.jobFilePath))
        {
            while (!sr.EndOfStream)
            {
                jobs.Add(JsonConvert.DeserializeObject<Job>(sr.ReadLine()));
            }
        }

        return jobs;
    }
}
