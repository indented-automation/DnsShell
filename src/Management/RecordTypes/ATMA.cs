using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class ATMA : ResourceRecord
    {
        public ATMAFormat Format { get; private set; }
        public String ATMAddress { get; private set; }

        internal ATMA(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            base.RecordType = RecordType.ATMA;

            this.Format = (ATMAFormat)wmiRecord.Properties["Format"].Value;
            this.ATMAddress = (String)wmiRecord.Properties["ATMAddress"].Value;
        }
   }
}
