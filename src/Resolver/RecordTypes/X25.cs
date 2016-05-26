using System;

namespace DnsShell.Resolver
{
    public class X25 : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                PSDNADDRESS                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String PSDNAddress;

        private Byte Length;

        internal X25(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            Length = Reader.ReadByte();
            PSDNAddress = Reader.ReadChars(Length).ToString();

            base.RecordData = PSDNAddress;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
