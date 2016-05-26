using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class WINSR : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                  LOCAL FLAG                   |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                LOOKUP TIMEOUT                 |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                 CACHE TIMEOUT                 |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |               NUMBER OF SERVERS               |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                  DOMAIN NAME                  /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public WINSMappingFlag MappingFlag { get; private set; }
        public UInt32 LookupTimeout { get; private set; }
        public UInt32 CacheTimeout { get; private set; }
        public String ResultDomain { get; private set; }

        internal WINSR(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            MappingFlag = (WINSMappingFlag)Reader.ReadUInt32(true);
            LookupTimeout = Reader.ReadUInt32(true);
            CacheTimeout = Reader.ReadUInt32(true);

            ResultDomain = DnsPacket.DecodeName(Reader);

            String RecordData = String.Format("L{0} C{1} ( {2} )",
                LookupTimeout,
                CacheTimeout,
                ResultDomain);
            if (MappingFlag == WINSMappingFlag.NoReplication) { RecordData = "LOCAL " + RecordData; }

            base.RecordData = RecordData;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
