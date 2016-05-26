using System;
using System.Collections;
using System.Text;
using System.IO;

namespace DnsShell.Resolver
{
    public class ResourceRecord
    {
        //                                   1  1  1  1  1  1
        //     0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                                               |
        //    /                                               /
        //    /                      NAME                     /
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                      TYPE                     |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     CLASS                     |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                      TTL                      |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                   RDLENGTH                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
        //    /                     RDATA                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String Name { get; internal set; }
        public UInt32 TTL { get; internal set; }
        public RecordType RecordType { get; internal set; }
        public RecordClass RecordClass { get; internal set; }
        public String RecordData { get; internal set; }

        internal UInt16 RDataLength;

        internal void UpdateProperties(EndianBinaryReader Reader)
        {
            this.Name = DnsPacket.DecodeName(Reader);
            this.RecordType = (RecordType)Reader.ReadUInt16(true);
            this.RecordClass = (RecordClass)Reader.ReadUInt16(true);
            this.TTL = Reader.ReadUInt32(true);
            this.RDataLength = Reader.ReadUInt16(true);
        }

        // Public Methods

        public override String ToString()
        {
            return String.Format("{0} {1} {2} {3} {4}",
                Name,
                TTL.ToString(),
                RecordClass.ToString(),
                RecordType.ToString(),
                RecordData);
        }
    }
}
