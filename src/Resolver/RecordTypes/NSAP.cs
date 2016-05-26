using System;

namespace DnsShell.Resolver
{
    class NSAP : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   PTRDNAME                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String DName;

        internal NSAP(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            DName = DnsPacket.DecodeName(Reader);

            base.RecordData = DName;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
