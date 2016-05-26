using System;

namespace DnsShell.Resolver
{
    public class RT : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                  PREFERENCE                   |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /               INTERMEDIATEHOST                /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 Preference { get; private set; }
        public String TargetName { get; private set; }

        internal RT(EndianBinaryReader Reader)
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
