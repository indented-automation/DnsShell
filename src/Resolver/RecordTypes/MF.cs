using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class MF : ResourceRecord
    {
        //    OBSOLETE
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   MADNAME                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String Hostname;

        internal MF(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);
            
            Hostname = DnsPacket.DecodeName(Reader);

            base.RecordData = Hostname.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
