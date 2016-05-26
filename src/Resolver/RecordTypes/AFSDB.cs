using System;
using System.Text;
using System.Net;
using System.IO;

namespace DnsShell.Resolver
{
    public class AFSDB : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    SUBTYPE                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    HOSTNAME                   /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 SubType { get; private set; }
        public AFSDBSubType SubTypeName { get; private set; }
        public String TargetName { get; private set; }

        internal AFSDB(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            SubType = Reader.ReadUInt16(true);
            if (Enum.IsDefined(typeof(AFSDBSubType), SubType))
            {
                SubTypeName = (AFSDBSubType)SubType;
            }
            TargetName = DnsPacket.DecodeName(Reader);

            base.RecordData = String.Format("{0} {1}", SubType, TargetName);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
