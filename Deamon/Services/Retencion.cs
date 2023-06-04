using Deamon.Backup;
using Deamon.Communication;
using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class Retencion
{
    private int retencion;
    private int packageLimit;
    private string path;
    private List<string> data = new List<string>();
    private Paths paths = new Paths();
    private protected Config config;

    public Retencion(int id, int destinationId, int retencion, int packageLimit, Config config)
    {
        this.path = paths.RoamingPath + @$"\Retencion_{destinationId}.txt";

        this.retencion = retencion;
        this.packageLimit = packageLimit;

        if (!File.Exists(this.path))
        {
            FileStream stream = File.Create(this.path);
            stream.Close();
        }

        this.config = config;
    }

    public void WriteRetencion(string path)
    {
        using (StreamWriter writer = new(this.path))
        {
            foreach (string item in data)
            {
                writer.WriteLine(item);
            }
            writer.WriteLine(path);
        }
    }

    public async Task ReadRetencion()
    {
        using (StreamReader reader = new StreamReader(this.path))
        {
            while (!reader.EndOfStream)
            {
                this.data.Add(reader.ReadLine());
            }


            if (retencion * packageLimit == data.Count)
            {
                //int limit = data.Count;
                for (int i = 0; i < packageLimit; i++)
                {
                    if (data[i].Substring(0, 4) == "ftp:")
                        await FTPDelete(data[i]);
                    else
                        LocalDelete();
                }                    
            }
        }
    }

    public void LocalDelete()
    {
        try
        {
            if (config.IsCompressed)
                File.Delete(data[0]);
            else
                Directory.Delete(data[0], true);

            data.RemoveAt(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not delete non-existent file");
        }
    }

    public async Task FTPDelete(string filepath)
    {
        FTPRegex regex = new();
        FTPdata data = await regex.FTPregex(filepath);

        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(data.server);  
        ftpRequest.Credentials = new NetworkCredential(data.username, data.password);   
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

        if (config.IsCompressed)
        {
            ftpRequest = (FtpWebRequest)WebRequest.Create(data.server + data.remoteFilePath);
            ftpRequest.Credentials = new NetworkCredential(data.username, data.password);    
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;                 

            FtpWebResponse deleteResponse = (FtpWebResponse)ftpRequest.GetResponse();
            await LogReport.AddReport("Delete completed: " + deleteResponse.StatusDescription);
        }
        else
        {
            ftpRequest = (FtpWebRequest)WebRequest.Create(data.server + data.remoteFilePath);
            ftpRequest.Credentials = new NetworkCredential(data.username, data.password);
            ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

            FtpWebResponse deleteResponse = (FtpWebResponse)ftpRequest.GetResponse();
            await LogReport.AddReport("Delete completed:: " + deleteResponse.StatusDescription);
        }

        this.data.RemoveAt(0);
    }
}
