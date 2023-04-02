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
    private Paths paths = new Paths();

    public FileGetter()
    {
        if (!File.Exists(paths.JobsPath))
        {
            FileStream filestream = File.Create(paths.JobsPath);
            filestream.Close();
        }

        if (!File.Exists(paths.UsersPath))
        {
            FileStream fileStream = File.Create(paths.UsersPath);
            fileStream.Close();
        }
        
    }

    public int? GetID()
    {
        if (!File.Exists(paths.IDPath))
        { 
            FileStream stream = File.Create(paths.IDPath);
            stream.Close();

            using (StreamWriter sw = new StreamWriter(paths.IDPath))
            {
                sw.Write(7.ToString()); // vytvoří file s ID 7. Zatím statické s funkčím webem doplním registraci
            }

            return null;
        }
        else
        {
            using (StreamReader sr = new StreamReader(paths.IDPath))
            {
                int ID;
                try
                {
                    ID = Convert.ToInt32(sr.ReadLine());
                }
                catch (Exception)
                {
                    File.Delete(paths.IDPath);
                    return null;
                }
                return ID;
            }
        }
    }

    public void SaveJobsToFile(string json)
    {
        using(StreamWriter sw = new StreamWriter(paths.JobsPath))
        {
            sw.WriteLine(json);
        }
    }

    public Job GetJobsFromFile()
    {
        Job job = new();

        using(StreamReader sr = new StreamReader(paths.JobsPath))
        {
            job = JsonConvert.DeserializeObject<Job>(sr.ReadToEnd());
        }

        return job;
    }

    public void SaveUsersToFile(string json)
    {
        using (StreamWriter sw = new StreamWriter(paths.UsersPath))
        {
            sw.WriteLine(json);
        }
    }

    public List<User> GetUsersFromFile()
    {
        List<User> users = new List<User>();

        using (StreamReader sr = new StreamReader(paths.UsersPath))
        {
            users = JsonConvert.DeserializeObject<List<User>>(sr.ReadToEnd());
        }

        return users;
    }
}
