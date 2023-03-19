using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deamon.Communication;
public class GetAddresses
{
    public List<string> GetIpAddresses()
    {
        List<string> ips = new List<string>();

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress iP in host.AddressList)
        {
            Regex redex = new Regex("([0-9]{1,3}[.]){3}[0-9]{1,3}");
            if (redex.IsMatch(iP.ToString()))
            {
                ips.Add(iP.ToString());
            }
        }

        return ips;
    }
}
