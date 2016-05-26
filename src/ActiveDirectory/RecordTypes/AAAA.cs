using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class AAAA : ResourceRecord
    {
        public IPAddress IPAddress { get; private set; }

        internal AAAA(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);
            this.IPAddress = base.RecordReader.ReadIPv6Address();
            base.RecordData = this.IPAddress.ToString();
        }
    }
}
