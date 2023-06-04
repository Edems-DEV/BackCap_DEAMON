using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Models;
public class FTPdata
{
    public string server { get; set; }
    public string remoteFilePath { get; set; }
    public string username { get; set; }
    public string password { get; set; }

    public FTPdata(string server, string remoteFilePath, string username, string password)
    {
        this.server = server;
        this.remoteFilePath = remoteFilePath;
        this.username = username;
        this.password = password;
    }

}
