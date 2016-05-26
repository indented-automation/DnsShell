using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.IO;

namespace DnsShell.ActiveDirectory
{
    public class ResourceRecord
    {
        //                                 1  1  1  1  1  1
        //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                 RDATA LENGTH                  |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                      TYPE                     |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |        VERSION        |         RANK          |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                     FLAGS                     |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                 UPDATEDATSERIAL               |
        //  |                                               |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                      TTL                      |
        //  |                                               |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                    RESERVED                   |
        //  |                                               |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //  |                   TIMESTAMP                   |
        //  |                                               |
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
        //  /                     RDATA                     /
        //  /                                               /
        //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String DN { get; internal set; }
        public String Name { get; internal set; }

        public UInt16 RDataLength { get; internal set; }
        public RecordType RecordType { get; internal set; }
        public RecordClass RecordClass = RecordClass.IN;
        public String RecordData { get; internal set; }
        public Byte Version { get; internal set; }
        public Rank Rank { get; internal set; }
        public UInt16 Flags { get; internal set; }
        public UInt32 UpdatedAtSerial { get; internal set; }
        public UInt32 TTL { get; internal set; }
        public UInt32 Reserved { get; internal set; }
        public Object TimeStamp { get; internal set; }

        public Guid ObjectGuid { get; internal set; }
        public DateTime WhenChanged { get; internal set; }
        public DateTime WhenCreated { get; internal set; }

        public Boolean IsSharedNode { get; internal set; }

        internal EndianBinaryReader RecordReader;

        private void ReadDnsRecord()
        {
            this.RDataLength = RecordReader.ReadUInt16();
            this.RecordType = (RecordType)RecordReader.ReadUInt16();
            this.Version = RecordReader.ReadByte();
            this.Rank = (Rank)RecordReader.ReadByte();
            this.Flags = RecordReader.ReadUInt16();
            this.UpdatedAtSerial = RecordReader.ReadUInt32();
            this.TTL = RecordReader.ReadUInt32(true);
            this.Reserved = RecordReader.ReadUInt32();
            UInt32 TimeStampValue = RecordReader.ReadUInt32();
            if (TimeStampValue > 0)
            {
                this.TimeStamp = new DateTime(1601, 1, 1).AddHours(TimeStampValue);
            }
            else
            {
                this.TimeStamp = "Static";
            }
        }

        internal void UpdateProperties(SearchResultEntry Entry, int Index = 0)
        {
            this.DN = Entry.DistinguishedName;
            this.Name = (String)Entry.Attributes["name"].GetValues(typeof(String))[0];

            this.ObjectGuid = new Guid((Byte[])Entry.Attributes["objectguid"].GetValues(typeof(Byte[]))[0]);

            this.WhenChanged = DateTime.ParseExact(
                (String)Entry.Attributes["whenchanged"].GetValues(typeof(String))[0], 
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture);

            this.WhenCreated = DateTime.ParseExact(
                (String)Entry.Attributes["whencreated"].GetValues(typeof(String))[0],
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();

            this.IsSharedNode = false;
            if (Entry.Attributes["dnsrecord"].Count > 1)
            {
                this.IsSharedNode = true;
            }

            MemoryStream DnsRecordStream = new MemoryStream((Byte[])Entry.Attributes["dnsrecord"].GetValues(typeof(Byte[]))[Index]);
            this.RecordReader = new EndianBinaryReader(DnsRecordStream);
            this.ReadDnsRecord();
        }

        internal void UpdateProperties(DirectoryEntry Entry, int Index = 0)
        {
            this.DN = Entry.Properties["distinguishedname"][0].ToString();
            this.Name = Entry.Properties["name"][0].ToString();

            this.ObjectGuid = (Guid)Entry.Properties["objectguid"][0];

            this.WhenChanged = (DateTime)Entry.Properties["whenchanged"][0];
            this.WhenCreated = (DateTime)Entry.Properties["whencreated"][0];

            this.IsSharedNode = false;
            if (Entry.Properties["dnsrecord"].Count > 1)
            {
                this.IsSharedNode = true;
            }

            MemoryStream DnsRecordStream = new MemoryStream(((Byte[])Entry.Properties["dnsrecord"][Index]));
            this.RecordReader = new EndianBinaryReader(DnsRecordStream);
            this.ReadDnsRecord();
        }

        internal String ReadName()
        {
            //                                  1  1  1  1  1  1
            //    0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            //  |         LENGTH        |   NUMBER OF LABELS    |
            //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            //  |      LABEL LENGTH     |                       |
            //  |--+--+--+--+--+--+--+--+                       |
            //  /                     DATA                      /
            //  /                                               /
            //  +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

            Byte Length = this.RecordReader.ReadByte();
            Byte NumLabels = this.RecordReader.ReadByte();

            String[] Labels = new String[NumLabels];

            for (int i = 0; i < NumLabels; i++)
            {
                Byte LabelLength = this.RecordReader.ReadByte();
                Labels[i] =  new String(this.RecordReader.ReadChars(LabelLength));
            }
            // Drop the terminating character
            this.RecordReader.ReadByte();
            return String.Join(".", Labels);
        }
        
        public override string ToString()
        {
            return String.Format("{0} {1} IN {2} {3}", this.Name, this.TTL, this.RecordType, this.RecordData);
        }

        internal Byte[] RRToByte()
        {
            Byte[] RRBytes = new Byte[24];

            UInt32 RawTimeStamp = 0;
            if (!(this.TimeStamp is String))
            {
                RawTimeStamp = (UInt32)((DateTime)this.TimeStamp - (new DateTime(1601, 1, 1))).TotalHours;
            }

            Array.Copy(EndianBitConverter.ToByte((UInt16)this.RecordType, true), 0, RRBytes, 2, 2);
            RRBytes[4] = 5;
            RRBytes[5] = (Byte)this.Rank;
            Array.Copy(EndianBitConverter.ToByte((UInt32)this.UpdatedAtSerial, true), 0, RRBytes, 8, 4);
            Array.Copy(EndianBitConverter.ToByte((UInt32)this.TTL, false), 0, RRBytes, 12, 4);
            Array.Copy(EndianBitConverter.ToByte(RawTimeStamp, true), 0, RRBytes, 20, 4);

            return RRBytes;
        }
    }
}
