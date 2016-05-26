using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class ISDN : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                ISDNADDRESS                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                 SUBADDRESS                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String ISDNAddress { get; private set; }
        public String SubAddress { get; private set; }

        private Byte ISDNAddressLength;
        private Byte SubAddressLength;

        internal ISDN(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            ISDNAddressLength = Reader.ReadByte();
            ISDNAddress = new String(Reader.ReadChars(ISDNAddressLength));
            SubAddressLength = Reader.ReadByte();
            SubAddress = new String(Reader.ReadChars(SubAddressLength));

            base.RecordData = String.Format("\"{0}\" \"{1}\"", ISDNAddress, SubAddress);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
