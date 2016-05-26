using System;

namespace DnsShell.Resolver
{
    public class TXT : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   TXT-DATA                    /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String Text;

        private Byte Length;

        internal TXT(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            Length = Reader.ReadByte();
            Text = new String(Reader.ReadChars(Length));

            base.RecordData = Text;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
