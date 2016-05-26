using System;
using System.Text;
using System.Net;
using System.IO;

namespace DnsShell.Resolver
{
    public class SRV : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                   PRIORITY                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    WEIGHT                     |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     PORT                      |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    TARGET                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 Priority { get; private set; }
        public UInt16 Weight { get; private set; }
        public UInt16 Port { get; private set; }
        public String TargetName { get; private set; }

        internal SRV(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            Priority = Reader.ReadUInt16(true);
            Weight = Reader.ReadUInt16(true);
            Port = Reader.ReadUInt16(true);
            TargetName = DnsPacket.DecodeName(Reader);

            base.RecordData = String.Format("{0} {1} {2} {3}",
                Priority,
                Weight,
                Port,
                TargetName);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
