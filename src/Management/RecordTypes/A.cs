using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class A : ResourceRecord
    {
        public IPAddress IPAddress { get; private set; }

        internal A(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.A;
            this.IPAddress = IPAddress.Parse((String)wmiRecord.Properties["IPAddress"].Value);
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String IPAddress = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (IPAddress != String.Empty & IPAddress != this.IPAddress.ToString())
            {
                inParams["IPAddress"] = IPAddress;
            }
            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
