using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class SOA : ResourceRecord
    {
        public String TargetName { get; private set; }
        public String ResponsiblePerson { get; private set; }
        public UInt32 Serial { get; private set; }
        public UInt32 Refresh { get; private set; }
        public UInt32 Retry { get; private set; }
        public UInt32 Expire { get; private set; }
        public UInt32 MinimumTTL { get; private set; }

        internal SOA(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            Serial = base.RecordReader.ReadUInt32();
            Refresh = base.RecordReader.ReadUInt32();
            Retry = base.RecordReader.ReadUInt32();
            Expire = base.RecordReader.ReadUInt32();
            MinimumTTL = base.RecordReader.ReadUInt32();
            TargetName = base.ReadName();
            ResponsiblePerson = base.ReadName();

            base.RecordData = String.Format("{0} {1} {2} {3} {4} {5} {6}",
                TargetName,
                ResponsiblePerson,
                Serial,
                Refresh,
                Retry,
                Expire,
                MinimumTTL);
        }
    }
}
