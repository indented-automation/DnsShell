using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class PTR : ResourceRecord
    {
        public String Hostname { get; private set; }

        internal PTR(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.PTR;
            this.Hostname = (String)wmiRecord.Properties["PTRDomainName"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String Hostname = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Hostname != String.Empty & Hostname != this.Hostname)
            {
                inParams["PTRDomainName"] = Hostname;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
