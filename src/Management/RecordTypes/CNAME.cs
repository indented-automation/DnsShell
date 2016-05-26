using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class CNAME : ResourceRecord
    {
        public String Hostname { get; private set; }

        internal CNAME(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.CNAME;
            this.Hostname = (String)wmiRecord.Properties["PrimaryName"].Value;
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
                inParams["PrimaryName"] = Hostname;
            }
            
            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
