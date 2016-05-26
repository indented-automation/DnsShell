using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class NS : ResourceRecord
    {
        public String Hostname { get; private set; }

        internal NS(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            Hostname = base.ReadName();

            base.RecordData = this.Hostname;
        }
    }
}
