using System;

namespace DnsShell.Resolver
{
    public class PTR : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   PTRDNAME                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String Hostname { get; private set; }

        internal PTR(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            Hostname = DnsPacket.DecodeName(Reader);

            base.RecordData = Hostname;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
