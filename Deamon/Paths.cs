using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deamon;
public class Paths
{
    public readonly string RoamingPath = $@"C:\Users\cyril\AppData\Roaming";
    public readonly string SnapchotNumberPath;
    public readonly string JobsPath;
    public readonly string IDPath;
    public readonly string UsersPath;
    public readonly string ServerPath = $"/api/Jobs/Machine/";

    public Paths()
    {
        SnapchotNumberPath = Path.Combine(RoamingPath, "SnapchotNumber.txt");
        JobsPath = Path.Combine(RoamingPath, "Jobs.txt");
        IDPath = Path.Combine(RoamingPath, "ID.txt");
        UsersPath = Path.Combine(RoamingPath, "Users.txt");
    }
}
