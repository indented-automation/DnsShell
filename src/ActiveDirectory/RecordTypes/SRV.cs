using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class SRV : ResourceRecord
    {
        public UInt16 Priority { get; private set; }
        public UInt16 Weight { get; private set; }
        public UInt16 Port { get; private set; }
        public String TargetName { get; private set; }

        internal SRV(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            Priority = base.RecordReader.ReadUInt16(true);
            Weight = base.RecordReader.ReadUInt16(true);
            Port = base.RecordReader.ReadUInt16(true);
            TargetName = base.ReadName();

            base.RecordData = String.Format("{0} {1} {2} {3}",
                Priority,
                Weight,
                Port,
                TargetName);
        }
    }
}
