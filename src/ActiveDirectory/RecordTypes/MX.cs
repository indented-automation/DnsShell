using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class MX : ResourceRecord
    {
        public UInt16 Preference { get; private set; }
        public String TargetName { get; private set; }

        internal MX(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            Preference = base.RecordReader.ReadUInt16(true);
            TargetName = base.ReadName();

            base.RecordData = String.Format("{0} {1}", Preference, TargetName);
        }
    }
}
