using Deamon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Deamon.Services;
public class MachineManager
{
    public MachineDto GetLocalMachine()
    {
        return new MachineDto()
        {
            Name = Environment.MachineName.ToString(),
            Description = this.GetDescription(),
            Os = Environment.OSVersion.ToString().Substring(0, 20),
            Ip_Address = this.GetLocalIPAddress(),
            Mac_Address = BitConverter.ToString(NetworkInterface
                    .GetAllNetworkInterfaces()
                    .FirstOrDefault()!
                    .GetPhysicalAddress()
                    .GetAddressBytes())
        };
    }

    public string GetLocalIPAddress()
    {
        foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Only consider Ethernet network interfaces (to exclude virtual interfaces, loopback, etc.)
            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet && netInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
                {
                    // Only consider IPv4 addresses
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        return null;
    }

    public string GetDescription()
    {
        return "To Do";
    }
}
