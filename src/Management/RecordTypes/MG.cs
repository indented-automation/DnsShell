using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class MG : ResourceRecord
    {
        public String MailboxName { get; private set; }

        internal MG(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.MG;

            this.MailboxName = (String)wmiRecord.Properties["MGMailbox"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String MailboxName = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (MailboxName != String.Empty & MailboxName != this.MailboxName)
            {
                inParams["MGMailbox"] = MailboxName;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
