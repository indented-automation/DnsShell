using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class SOA : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     MNAME                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                     RNAME                     /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    SERIAL                     |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    REFRESH                    |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     RETRY                     |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    EXPIRE                     |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    MINIMUM                    |
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String TargetName { get; private set; }
        public String ResponsiblePerson { get; private set; }
        public UInt32 Serial { get; internal set; }
        public UInt32 Refresh { get; private set; }
        public UInt32 Retry { get; private set; }
        public UInt32 Expire { get; private set; }
        public UInt32 MinimumTTL { get; private set; }

        internal SOA(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            TargetName = DnsPacket.DecodeName(Reader);
            ResponsiblePerson = DnsPacket.DecodeName(Reader);
            Serial = Reader.ReadUInt32(true);
            Refresh = Reader.ReadUInt32(true);
            Retry = Reader.ReadUInt32(true);
            Expire = Reader.ReadUInt32(true);
            MinimumTTL = Reader.ReadUInt32(true);

            base.RecordData = String.Format("{0} {1} {2} {3} {4} {5} {6}",
                TargetName,
                ResponsiblePerson,
                Serial,
                Refresh,
                Retry,
                Expire,
                MinimumTTL);
        }

        internal SOA(String Name, UInt32 Serial)
        {
            base.RecordClass = RecordClass.IN;
            base.RecordType = RecordType.SOA;
            base.Name = Name;
            this.Serial = Serial;
        }

        internal Byte[] ToIxfrByte()
        {
            Byte[] NameBytes = DnsPacket.EncodeName(base.Name.TrimEnd(new char[] { '.' }));
            UInt16 RecordDataLength = 22;
            Int32 TotalLength = NameBytes.Length + 32;

            Int32 CurrentIndex = 0;

            // Create the Byte Array
            Byte[] RecordBytes = new Byte[TotalLength];

            // Populate the Byte Array
            Array.Copy(NameBytes, RecordBytes, NameBytes.Length);
            CurrentIndex += NameBytes.Length;
            Array.Copy(EndianBitConverter.ToByte((UInt16)base.RecordType, false), 0, RecordBytes, CurrentIndex, 2);
            CurrentIndex += 2;
            Array.Copy(EndianBitConverter.ToByte((UInt16)base.RecordClass, false), 0, RecordBytes, CurrentIndex, 2);
            CurrentIndex += 6;
            Array.Copy(EndianBitConverter.ToByte(RecordDataLength, false), 0, RecordBytes, CurrentIndex, 2);
            CurrentIndex += 4;
            Array.Copy(EndianBitConverter.ToByte(Serial, false), 0, RecordBytes, CurrentIndex, 4);

            return RecordBytes;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
