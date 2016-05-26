using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class WINS : ResourceRecord
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
        //    /                SERVER IP LIST                 /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public WINSMappingFlag MappingFlag { get; private set; }
        public UInt32 LookupTimeout { get; private set; }
        public UInt32 CacheTimeout { get; private set; }
        public String WinsServers { get; private set; }

        private UInt32 NumberOfServers;
        private IPAddress[] ServerList;

        internal WINS(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            MappingFlag = (WINSMappingFlag)Reader.ReadUInt32(true);
            LookupTimeout = Reader.ReadUInt32(true);
            CacheTimeout = Reader.ReadUInt32(true);
            NumberOfServers = Reader.ReadUInt32(true);

            ServerList = new IPAddress[NumberOfServers];

            for (Int32 i = 0; i < NumberOfServers; i++)
            {
                ServerList[i] = Reader.ReadIPAddress();
            }

            String RecordData = String.Format("L{0} C{1} ( {2} )",
                LookupTimeout,
                CacheTimeout,
                String.Join(" ",
                    Array.ConvertAll(ServerList, new Converter<IPAddress, String>(IPToString))));
            if (MappingFlag == WINSMappingFlag.NoReplication) { RecordData = "LOCAL " + RecordData; }

            base.RecordData = RecordData;
        }

        private String IPToString(IPAddress IPAddress)
        {
            return IPAddress.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
