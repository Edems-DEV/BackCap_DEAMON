using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Deamon;
internal class _delete_FTPreference
{
    public void FTPreference()
    {
        string server = "ftp://example.com";
        string username = "your-username";
        string password = "your-password";

        string localFilePath = "C:\\path\\to\\local\\file.txt";               //to co budeme kopírovat
        string remoteFilePath = "/path/to/remote/file.txt";                   //to kam to budeme kopírovat (mi to budeme mít už rovnou za servererm)

        // Connect to the FTP server
        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(server);  //vytvoří request
        ftpRequest.Credentials = new NetworkCredential(username, password);   //přihlášení pomocí údajů
        ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;              //???

        FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();//získání odpovědi od FTP
        Stream responseStream = ftpResponse.GetResponseStream();              //do třídy Strem se přečte ten response
        StreamReader reader = new StreamReader(responseStream);               //streamreader to začne číst

        // List directory contents
        Console.WriteLine("Directory List:");
        Console.WriteLine(reader.ReadToEnd());                                //vypíše co přečetl


        // ------------------------------------------------------------------- UPLOAD FILE ----------------------------------------------------
        ftpRequest = (FtpWebRequest)WebRequest.Create(server + remoteFilePath);//vytvoří request
        ftpRequest.Credentials = new NetworkCredential(username, password);   //přihlášení pomocí údajů
        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;                 //???

        byte[] fileContents = File.ReadAllBytes(localFilePath);               //zjistí to guts toho filu
        ftpRequest.ContentLength = fileContents.Length;                       //+ délku

        Stream requestStream = ftpRequest.GetRequestStream();                 //uloží si ftp Stream
        requestStream.Write(fileContents, 0, fileContents.Length);            //do toho zapíše guts toho filu (data, offset (0), délka)
        requestStream.Close();                                                //uzavře

        FtpWebResponse uploadResponse = (FtpWebResponse)ftpRequest.GetResponse();//NAHRAJE DATA Z REQUESTU
        Console.WriteLine("Upload File Complete. Status: " + uploadResponse.StatusDescription);

        // ------------------------------------------------------------------- DELETE FILE ----------------------------------------------------
        ftpRequest = (FtpWebRequest)WebRequest.Create(server + remoteFilePath);//vytvoří request
        ftpRequest.Credentials = new NetworkCredential(username, password);   //přihlášení pomocí údajů 
        ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;                 //???
         
        FtpWebResponse deleteResponse = (FtpWebResponse)ftpRequest.GetResponse();//data v requestu SMAŽE ??
        Console.WriteLine("Delete File Complete. Status: " + deleteResponse.StatusDescription);

        // Clean up
        reader.Close();
        responseStream.Close();
        ftpResponse.Close();
        uploadResponse.Close();
        deleteResponse.Close();

        Console.ReadLine();
    }
}
