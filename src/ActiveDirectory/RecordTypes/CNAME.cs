using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class CNAME : ResourceRecord
    {
        public String Hostname { get; private set; }

        internal CNAME(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);
            this.Hostname = base.ReadName();
            base.RecordData = this.Hostname;
        }
    }
}
