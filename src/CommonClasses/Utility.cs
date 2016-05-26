using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace DnsShell
{
    class Utility
    {
        internal static String[] GetSearchList()
        {
            string Query = "SELECT DnsDomainSuffixSearchOrder FROM Win32_NetworkAdapterConfiguration";
            ManagementObjectSearcher WMISearch = new ManagementObjectSearcher(Query);
            foreach (ManagementObject SearchResult in WMISearch.Get())
            {
                if (SearchResult.GetPropertyValue("DNSDomainSuffixSearchOrder") != null)
                {
                    return (string[])SearchResult.GetPropertyValue("DNSDomainSuffixSearchOrder");
                }
            }
            return new string[] { "" };
        }

        public static String[] GetDnsServer()
        {
            ArrayList Servers = new ArrayList();

            foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (Nic.OperationalStatus == OperationalStatus.Up & Nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    IPInterfaceProperties NicProperties = Nic.GetIPProperties();
                    foreach (IPAddress DnsAddress in NicProperties.DnsAddresses)
                    {
                        Servers.Add(DnsAddress.ToString());
                    }
                }
            }
            return (string[])Servers.ToArray(typeof(string));
        }
    }
}
