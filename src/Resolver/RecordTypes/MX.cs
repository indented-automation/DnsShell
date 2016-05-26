using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class MX : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                  PREFERENCE                   |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   EXCHANGE                    /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 Preference { get; private set; }
        public String TargetName { get; private set; }

        internal MX(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            Preference = Reader.ReadUInt16(true);
            TargetName = DnsPacket.DecodeName(Reader);

            base.RecordData = String.Format("{0} {1}", Preference, TargetName);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
