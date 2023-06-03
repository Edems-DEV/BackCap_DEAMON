using Deamon.Communication;
using Deamon.Models;
using Deamon.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

    public async virtual Task Backup()
    {
        await LogReport.AddReport("Backup started");        

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
            //TODO - bude ftp server součástí filepath ? (asi jo - kod je stím že ano)
            string filepath = Path.Combine(destiantion.DestPath, @$"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}");
            string ZIPfilepath = filepath + ".zip";
                
            Retencion retencion = new Retencion(config.Id, destiantion.Id, config.Retention, config.PackageSize,config);
            retencion.ReadRetencion();

            foreach (Sources source in config.Sources)
            {
                List<string> different = new List<string>();
                try
                {
                    different = await DirectoryRead(source, json);
                }
                catch (Exception x)
                {
                    await LogReport.AddReport($"Deamon couldn't read data correctly ({x})");
                }
               

                try
                {
                    await DirectoryCreate(filepath, different, source);

                }
                catch (Exception x)
                {
                    await LogReport.AddReport($"Deamon couldn't copy data correctly ({x})");
                }
                
            }

            if(config.IsCompressed)
            {
                ZipFile.CreateFromDirectory(filepath, ZIPfilepath);
                Directory.Delete(filepath,true);
                retencion.WriteRetencion(ZIPfilepath);
            }
            else
                retencion.WriteRetencion(filepath);
        }

        UpdateSnapchot(await SourceToJson(json), path);
        await LogReport.AddReport("Backup finisehd succesfully !");
    }

    public async Task DirectoryCreate(string filepath, List<string> different, Sources source)
    {
        different.Sort();

        string server = string.Empty;
        string remoteFilePath = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        bool isFTP = false;

        if (source.Path.Substring(0,4) == "ftp:") //TODO neotestovano
        {
            string ftpUrl = "ftp://jon:apple@bigcompany.com/path/to/remote/file.txt";

            Regex regex = new Regex(@"^ftp://(?<username>\w+):(?<password>\w+)@(?<server>[\w.-]+)/(?<remoteFilePath>.+)$");
            Match match = regex.Match(ftpUrl);

            if (match.Success)
            {
                // Extract the components from the match groups
                server = "ftp://" + match.Groups["server"].Value;
                remoteFilePath = match.Groups["remoteFilePath"].Value;
                username = match.Groups["username"].Value;
                password = match.Groups["password"].Value;
            }
            else
                await LogReport.AddReport("Invalid FTP URL format.");
        }

        //FTP       

        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(server); 
        ftpRequest.Credentials = new NetworkCredential(username, password);
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

        FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
        Stream responseStream = ftpResponse.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        //FTP-

        if (different.Count == 0)
        {
            if(isFTP)
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(filepath);
                ftpRequest.Credentials = new NetworkCredential(username, password);   
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;                 
            }
            else
            {
                Directory.CreateDirectory(filepath);
            }
        }
        else
        {
            foreach (var item in different)
            {
                string result = item.Remove(0, source.Path.Length);                
                string destpath = filepath + result;

                if (isFTP)
                {

                    FtpWebRequest ftpRequestItemExists = (FtpWebRequest)WebRequest.Create(server + item);
                    ftpRequestItemExists.Credentials = new NetworkCredential(username, password);
                    ftpRequestItemExists.Method = WebRequestMethods.Ftp.GetDateTimestamp;

                    try
                    {
                        FtpWebResponse ftpResponseItemExists = (FtpWebResponse)ftpRequestItemExists.GetResponse(); //tady na tom to kdyžtak spadne
                        // Item exist?

                        ftpRequestItemExists = (FtpWebRequest)WebRequest.Create(server + item);
                        ftpRequestItemExists.Credentials = new NetworkCredential(username, password);
                        ftpRequestItemExists.Method = WebRequestMethods.Ftp.UploadFile;

                        ftpResponseItemExists.Close();
                    }
                    catch (WebException ex)
                    {
                        FtpWebResponse ftpResponseItemExists = (FtpWebResponse)ex.Response;
                        if (ftpResponseItemExists.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            ftpRequest = (FtpWebRequest)WebRequest.Create(server + remoteFilePath);
                            ftpRequest.Credentials = new NetworkCredential(username, password);   
                            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;                 

                            byte[] fileContents = File.ReadAllBytes(source.Path);                 
                            ftpRequest.ContentLength = fileContents.Length;                       

                            Stream requestStream = ftpRequest.GetRequestStream();                 
                            requestStream.Write(fileContents, 0, fileContents.Length);            
                            requestStream.Close();                                                

                            FtpWebResponse uploadResponse = (FtpWebResponse)ftpRequest.GetResponse();
                            await LogReport.AddReport("Upload File Complete. Status: " + uploadResponse.StatusDescription);
                        }
                        else
                        {
                            await LogReport.AddReport("An error occurred: " + ex.Message);
                        }
                        ftpResponseItemExists.Close();
                    }
                }
                else
                {
                    if (Directory.Exists(item))
                    {
                        Directory.CreateDirectory(destpath);
                    }
                    else
                    {
                        try
                        {
                            File.Copy(item, destpath);
                        }
                        catch (Exception x)
                        {
                            await LogReport.AddReport($"Deamon couldn't copy files ({x})");
                        }
                    }
                }                    
            }
        }        
    }

    public async Task<List<string>> DirectoryRead(Sources source,string json)
    {
        JsonConvertor convert = new JsonConvertor();
        Folder newDirectory = await convert.CreateStructrue(new Folder("A", source.Path));

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

    public async Task<string> SourceToJson(string json)
    {
        // vytvoření jsonu pro každý source. A jejich následná kombinace do jednoho 
        foreach (Sources source in config.Sources)
        {
            JsonCombiner jsonCombiner = new JsonCombiner();
            JsonConvertor jsonConvertor = new JsonConvertor();

            Folder newDirectory = await jsonConvertor.CreateStructrue(new Folder($"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}", source.Path));
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
