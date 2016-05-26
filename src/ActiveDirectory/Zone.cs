using System;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DnsShell.ActiveDirectory
{
    class Zone
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

        public String ZoneName { get; internal set; }
        public String DN { get; internal set; }
        public String DataFile { get; private set; }
        public IPAddress[] AllowNSRecordsAutoCreation { get; private set; }

        // Zone Type

        public ZoneType ZoneType { get; private set; }
        
        // Aging and Scavenging

        public Boolean Aging { get; private set; }
        public ZoneDynamicUpdate DynamicUpdate { get; private set; }
        public Object AgingEnabledDate { get; private set; }
        public Object NoRefreshInterval { get; private set; }
        public Object RefreshInterval { get; private set; }
        public IPAddress[] ScavengeServers { get; private set; }

        // Secondary Zones

        public IPAddress[] MasterServers { get; private set; }

        // Conditional Forwarders

        public Boolean ForwarderUseRecursion { get; private set; }

        // Miscellaneous

        public String DeletedFromHostname { get; private set; }
        public Object SecureTime { get; private set; }

        public Guid ObjectGuid { get; internal set; }
        public DateTime WhenChanged { get; internal set; }
        public DateTime WhenCreated { get; internal set; }

        internal Zone(SearchResultEntry Entry)
        {
            this.DN = Entry.DistinguishedName;
            this.ZoneName = (String)Entry.Attributes["name"].GetValues(typeof(String))[0];

            this.ObjectGuid = new Guid((Byte[])Entry.Attributes["objectguid"].GetValues(typeof(Byte[]))[0]);

            this.WhenChanged = DateTime.ParseExact(
                (String)Entry.Attributes["whenchanged"].GetValues(typeof(String))[0],
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture);

            this.WhenCreated = DateTime.ParseExact(
                (String)Entry.Attributes["whencreated"].GetValues(typeof(String))[0],
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
            
            for (int i = 0; i < Entry.Attributes["dnsproperty"].Count; i++)
            {
                MemoryStream DnsPropertyStream = new MemoryStream((Byte[])Entry.Attributes["dnsproperty"].GetValues(typeof(Byte[]))[i]);
                EndianBinaryReader PropertyReader = new EndianBinaryReader(DnsPropertyStream);
                ReadProperty(PropertyReader);
            }
        }

        private void ReadProperty(EndianBinaryReader PropertyReader)
        {
            UInt32 DataLength = PropertyReader.ReadUInt32();
            UInt32 NameLength = PropertyReader.ReadUInt32();
            // Must be 0
            UInt32 Flag = PropertyReader.ReadUInt32();
            // Must be 1
            UInt32 Version = PropertyReader.ReadUInt32();
            ZonePropertyID PropertyID = (ZonePropertyID)PropertyReader.ReadUInt32();

            UInt32 NumberOfServers;
            switch (PropertyID)
            {
                case ZonePropertyID.AgingEnabledTime:
                    UInt32 AgingEnabledDateHours = PropertyReader.ReadUInt32();
                    if (AgingEnabledDateHours > 0)
                    {
                        this.AgingEnabledDate = new DateTime(1601, 1, 1).AddHours(AgingEnabledDateHours);
                    }
                    break;
                case ZonePropertyID.AgingState:
                    this.Aging = false;
                    if ((UInt32)PropertyReader.ReadUInt32() == 1)
                    {
                        this.Aging = true;
                    }
                    break;
                case ZonePropertyID.AllowUpdate:
                    this.DynamicUpdate = (ZoneDynamicUpdate)PropertyReader.ReadByte();
                    break;
                case ZonePropertyID.AutoNSServers:
                    if (DataLength >= 4)
                    {
                        NumberOfServers = PropertyReader.ReadUInt32();
                        this.AllowNSRecordsAutoCreation = new IPAddress[NumberOfServers];
                        for (int i = 0; i < NumberOfServers; i++)
                        {
                            this.AllowNSRecordsAutoCreation[i] = PropertyReader.ReadIPAddress();
                        }
                    }
                    break;
                case ZonePropertyID.AutoNSServersDA:
                    // this.AllowNSRecordsAutoCreation
                    break;
                case ZonePropertyID.DCPromoConvert:
                    // Hide this property
                    break;
                case ZonePropertyID.DeletedFromHostname:
                    UnicodeEncoding UnicodeEncoding = new UnicodeEncoding();
                    this.DeletedFromHostname = UnicodeEncoding.GetString(PropertyReader.ReadBytes((Int32)DataLength));
                    break;
                case ZonePropertyID.MasterServers:
                    break;
                case ZonePropertyID.MasterServersDA:
                    UInt32 MaxCount = PropertyReader.ReadUInt32();
                    UInt32 AddrCount = PropertyReader.ReadUInt32();
                    Byte[] Stuff1 = PropertyReader.ReadBytes(24);

                    this.MasterServers = new IPAddress[AddrCount];

                    for (int i = 0; i < AddrCount; i++)
                    {
                        AddressFamily AddressFamily = (AddressFamily)PropertyReader.ReadUInt16();
                        UInt16 Port = PropertyReader.ReadUInt16(true);

                        IPAddress IPv4 = PropertyReader.ReadIPAddress();
                        IPAddress IPv6 = PropertyReader.ReadIPv6Address();

                        if (AddressFamily == AddressFamily.InterNetwork)
                        {
                            this.MasterServers[i] = IPv4;
                        }
                        else if (AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            this.MasterServers[i] = IPv6;
                        }

                        Byte[] Stuff2 = PropertyReader.ReadBytes(8);

                        UInt32 SALen = PropertyReader.ReadUInt32();

                        Byte[] Stuff3 = PropertyReader.ReadBytes(28);
                    }
                    break;
                case ZonePropertyID.NodeDBFlags:
                    break;
                case ZonePropertyID.NoRefreshInterval:
                    this.NoRefreshInterval = new TimeSpan((Int32)PropertyReader.ReadUInt32(), 0, 0);
                    break;
                case ZonePropertyID.RefreshInterval:
                    this.RefreshInterval = new TimeSpan((Int32)PropertyReader.ReadUInt32(), 0, 0);
                    break;
                case ZonePropertyID.ScavengingServers:
                    if (DataLength >= 4)
                    {
                        NumberOfServers = PropertyReader.ReadUInt32();
                        this.ScavengeServers = new IPAddress[NumberOfServers];
                        for (int i = 0; i < NumberOfServers; i++)
                        {
                            this.ScavengeServers[i] = PropertyReader.ReadIPAddress();
                        }
                    }
                    break;
                case ZonePropertyID.ScavengingServersDA:
                    //Console.WriteLine(String.Format("Property: {0} with DataLength: {1}, NameLength: {2} and Value: {3}", PropertyID, DataLength, NameLength, 0));
                    break;
                case ZonePropertyID.Securetime:
                    UInt64 SecuretimeSeconds = PropertyReader.ReadUInt64();
                    if (SecuretimeSeconds > 0)
                    {
                        this.SecureTime = new DateTime(1601, 1, 1).AddSeconds(SecuretimeSeconds);
                    }
                    break;
                case ZonePropertyID.Type:
                    this.ZoneType = (ZoneType)PropertyReader.ReadUInt32();
                    break;
            }
        }
    }
}
