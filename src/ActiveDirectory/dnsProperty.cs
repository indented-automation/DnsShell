using System;

namespace DnsShell.ActiveDirectory
{
    class dnsProperty
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                   DATALENGTH                  |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                   NAMELENGTH                  |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     FLAG                      |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    VERSION                    |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                      ID                       |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     DATA                      /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     NAME                      /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt32 DataLength { get; private set; }
        public UInt32 NameLength { get; private set; }
        public UInt32 Flag { get; private set; }
        public UInt32 Version { get; private set; }
        public ZonePropertyID PropertyID { get; private set; }
        public Object Data { get; private set; }

        public dnsProperty(EndianBinaryReader Property)
        {
            this.DataLength = Property.ReadUInt32(true);
            this.NameLength = Property.ReadUInt32(true);
            this.Flag = Property.ReadUInt32(true);
            this.Version = Property.ReadUInt32(true);
            this.PropertyID = (ZonePropertyID)Property.ReadUInt32(true);

            switch (this.PropertyID)
            {
                case ZonePropertyID.AgingEnabledTime: break;
                case ZonePropertyID.AgingState: break;
                case ZonePropertyID.AllowUpdate: break;
                case ZonePropertyID.AutoNSServers: break;
                case ZonePropertyID.AutoNSServersDA: break;
                case ZonePropertyID.DCPromoConvert:
                case ZonePropertyID.DeletedFromHostname:
                    Data = Property.ReadChars((Int32)this.DataLength);
                    break;
                case ZonePropertyID.MasterServers: break;
                case ZonePropertyID.MasterServersDA: break;
                case ZonePropertyID.NodeDBFlags: break;
                case ZonePropertyID.NoRefreshInterval: break;
                case ZonePropertyID.RefreshInterval: break;
                case ZonePropertyID.ScavengingServers: break;
                case ZonePropertyID.ScavengingServersDA: break;
                case ZonePropertyID.Securetime:
                    Data = Property.ReadUInt64();
                    break;
                case ZonePropertyID.Type: break;
            }
        }
    }
}
