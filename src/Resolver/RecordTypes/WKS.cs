using System;
using System.Net;
using System.Net.Sockets;

namespace DnsShell.Resolver
{
    public class WKS : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    ADDRESS                    |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |       PROTOCOL        |                       |
        //    +--+--+--+--+--+--+--+--+                       |
        //    |                                               |
        //    /                   <BIT MAP>                   /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public IPAddress Address { get; private set; }
        public ProtocolType IPProtocol { get; private set; }
        public String[] Services { get; private set; }

        private Byte[] BitMap;

        internal WKS(EndianBinaryReader Reader, UInt16 RDataLength)
        {
            base.UpdateProperties(Reader);

            Address = Reader.ReadIPAddress();
            IPProtocol = (ProtocolType)Reader.ReadByte();

            BitMap = new Byte[RDataLength - 5];
            for (Int32 i = 3; i < RDataLength; i++)
            {
                BitMap[i] = Reader.ReadByte();
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
