using System;

namespace DnsShell.Resolver
{
    public class RP : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     RNAME                     /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     DNAME                     /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String ResponsibleMailbox { get; private set; }
        public String TXTDomainName { get; private set; }

        internal RP(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            ResponsibleMailbox = DnsPacket.DecodeName(Reader);
            TXTDomainName = DnsPacket.DecodeName(Reader);

            base.RecordData = String.Format("{0} {1}", ResponsibleMailbox, TXTDomainName);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
