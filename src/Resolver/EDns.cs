using System;
using System.Collections;
using System.Text;
using System.Net;
using System.IO;
using DnsShell.Resolver;

namespace DnsShell.Resolver
{
    public class EDns
    {
        //                                   1  1  1  1  1  1
        //     0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                                               |
        //    /                                               /
        //    /                      NAME                     /
        //    |                                               |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                      TYPE                     |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    UDPSIZE                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |    EXTENDED-RCODE     |       VERSION         |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |DO|                    Z                       |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                 OPTIONLENGTH                  |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
        //    /                    OPTIONS                    /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 UdpSize { get; private set; }
        public Byte ExtendedRCode { get; private set; }
        public Byte Version { get; private set; }
        public EDnsFlag Flags { get; private set; }
        public UInt16 OptionLength;
        public EDnsOption[] Options { get; private set; }

        internal RecordType RecordType { get; private set; }
        internal String Name { get; private set; }

        internal EDns(EndianBinaryReader Reader)
        {
            this.Name = DnsPacket.DecodeName(Reader);
            this.RecordType = (RecordType)Reader.ReadUInt16(true);
            this.UdpSize = Reader.ReadUInt16(true);
            this.ExtendedRCode = Reader.ReadByte();
            this.Version = Reader.ReadByte();
            this.Flags = (EDnsFlag)Reader.ReadUInt16(true);

            this.OptionLength = Reader.ReadUInt16();

            if (this.OptionLength > 0)
            {
                ArrayList OptionsTemp = new ArrayList();

                UInt16 BytesRemaining = this.OptionLength;
                do
                {
                    EDnsOption Option = new EDnsOption(Reader);
                    BytesRemaining -= (UInt16)(Option.OptionLength + 4);

                    OptionsTemp.Add(Option);
                } while (BytesRemaining > 0);
                this.Options = (EDnsOption[])OptionsTemp.ToArray(typeof(EDnsOption));
            }
        }

        internal static Byte[] ToByte(UInt16 UdpSize, UInt16 OptionLength)
        {
            Byte[] RecordBytes = new Byte[11];

            Array.Copy(EndianBitConverter.ToByte((UInt16)RecordType.OPT, false), 0, RecordBytes, 1, 2);
            Array.Copy(EndianBitConverter.ToByte((UInt16)UdpSize, false), 0, RecordBytes, 3, 2);
            Array.Copy(EndianBitConverter.ToByte((UInt16)OptionLength, false), 0, RecordBytes, 9, 2);

            return RecordBytes;
        }

        public override string ToString()
        {
            return String.Format("Version: {0} Flags: {1} UdpSize: {2}",
                this.Version,
                this.Flags.ToString(),
                this.UdpSize);
        }
    }

    public class EDnsOption
    {
        //                                   1  1  1  1  1  1
        //     0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                  OPTION-CODE                  |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                 OPTION-LENGTH                 |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                  OPTION-DATA                  /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public EDnsOptionName OptionName { get; private set; }
        public Byte[] OptionData { get; private set; }

        private UInt16 OptionCode;
        internal UInt16 OptionLength;

        internal EDnsOption(EndianBinaryReader Reader)
        {
            this.OptionCode = Reader.ReadUInt16(true);
            this.OptionLength = Reader.ReadUInt16(true);

            if (this.OptionLength > 0)
            {
                this.OptionData = Reader.ReadBytes(this.OptionLength);
            }
        }

        internal static Byte[] NSIDOption()
        {
            // A blank NSID option byte array

            Byte[] EDnsOption = new Byte[4];
            Array.Copy(EndianBitConverter.ToByte((UInt16)EDnsOptionName.NSID, false), EDnsOption, 2);

            return EDnsOption;
        }
    }
}
