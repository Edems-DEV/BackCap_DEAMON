using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deamon.Services;
public class FTPRegex
{
    public async Task<FTPdata> FTPregex(string filepath)
    {
        Regex regex = new Regex(@"^ftp://(?<username>\w+):(?<password>\w+)@(?<server>[\w.-]+)/(?<remoteFilePath>.+)$");
        Match match = regex.Match(filepath);

        if (!match.Success)
            throw new Exception("Invalid FTP URL format.");

        FTPdata data;

        string server = "ftp://" + match.Groups["server"].Value;
        string remoteFilePath = match.Groups["remoteFilePath"].Value);
        string username = match.Groups["username"].Value;
        string password = match.Groups["password"].Value;

        return data = new FTPdata(server, remoteFilePath, username, password);
    }
}
