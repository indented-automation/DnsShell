using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class MINFO : ResourceRecord
    {
        public String ResponsibleMailbox { get; private set; }
        public String ErrorMailbox { get; private set; }

        internal MINFO(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.MINFO;
            this.ResponsibleMailbox = (String)wmiRecord.Properties["ResponsibleMailbox"].Value;
            this.ErrorMailbox = (String)wmiRecord.Properties["ErrorMailbox"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String ResponsibleMailbox = "",
            String ErrorMailbox = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (ResponsibleMailbox != String.Empty & ResponsibleMailbox != this.ResponsibleMailbox)
            {
                inParams["ResponsibleMailbox"] = ResponsibleMailbox;
            }
            if (ErrorMailbox != String.Empty & ErrorMailbox != this.ErrorMailbox)
            {
                inParams["ErrorMailbox"] = ErrorMailbox;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
