using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class NXT : ResourceRecord
    {
        public String NextDomainName { get; private set; }
        public String Types { get; private set; }

        internal NXT(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.NXT;
            this.NextDomainName = (String)wmiRecord.Properties["NextDomainName"].Value;
            this.Types = (String)wmiRecord.Properties["Types"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String NextDomainName = "",
            String Types = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (NextDomainName != String.Empty & NextDomainName != this.NextDomainName)
            {
                inParams["NextDomainName"] = NextDomainName;
            }
            if (Types != String.Empty & Types != this.Types)
            {
                inParams["Types"] = Types;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
