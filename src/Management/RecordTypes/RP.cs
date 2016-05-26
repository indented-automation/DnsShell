using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class RP : ResourceRecord
    {
        public String ResponsibleMailbox { get; private set; }
        public String TXTDomainName { get; private set; }

        internal RP(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.RP;
            this.ResponsibleMailbox = (String)wmiRecord.Properties["RPMailbox"].Value;
            this.TXTDomainName = (String)wmiRecord.Properties["TXTDomainName"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String ResponsibleMailbox = "",
            String TXTDomainName = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (ResponsibleMailbox != String.Empty & ResponsibleMailbox != this.ResponsibleMailbox)
            {
                inParams["PTRDomainName"] = ResponsibleMailbox;
            }
            if (TXTDomainName != String.Empty & TXTDomainName != this.TXTDomainName)
            {
                inParams["TXTDomainName"] = TXTDomainName;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
