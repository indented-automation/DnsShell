using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class MINFO : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    RMAILBX                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    EMAILBX                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String ResponsibleMailbox;
        public String ErrorMailbox;

        internal MINFO(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            ResponsibleMailbox = DnsPacket.DecodeName(Reader);
            ErrorMailbox = DnsPacket.DecodeName(Reader);

            base.RecordData = String.Format("{0} {1}", ResponsibleMailbox, ErrorMailbox);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
