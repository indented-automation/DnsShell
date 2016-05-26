using System;
using System.Collections;
using System.Text;
using System.IO;

namespace DnsShell.Resolver
{
    // DnsShell.Resolver.Question - Contains methods to build and parse the Question section from a DNS Packet.
    //
    // Constructors:
    //   Public: Question(string Name, string RRType)
    //   Public: Question(string Name, string RRType, string RRClass)
    //   Public: Question(Reply Response)
    //
    // Methods:
    //   Public: ToByteArray()
    //   Public Override: ToString()

    public class Question
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                                               |
        //    /                     QNAME                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     QTYPE                     |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                     QCLASS                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String Name;
        public RecordType RecordType = RecordType.A;
        public RecordClass RecordClass = RecordClass.IN;

        private Byte[] EncodedName;

        // Constructors

        internal Question(String Name, RecordType RecordType)
        {
            this.Name = Name.TrimEnd(new char[] { '.' });
            this.RecordType = RecordType;
            this.EncodedName = DnsPacket.EncodeName(this.Name);
        }

        internal Question(String Name, RecordType RecordType, RecordClass RecordClass)
        {
            this.Name = Name;
            this.RecordType = RecordType;
            this.RecordClass = RecordClass;
            this.EncodedName = DnsPacket.EncodeName(Name);
        }

        internal Question(EndianBinaryReader Response)
        {
            this.Name = DnsPacket.DecodeName(Response);
            this.RecordType = (RecordType)(Response.ReadUInt16(true));
            this.RecordClass = (RecordClass)(Response.ReadUInt16(true));
        }

        internal Byte[] ToByte()
        {
            Byte[] QuestionBytes = new Byte[this.EncodedName.Length + 4];

            Array.Copy(this.EncodedName, QuestionBytes, this.EncodedName.Length);
            Array.Copy(
                EndianBitConverter.ToByte((UInt16)RecordType, false),
                0, QuestionBytes, this.EncodedName.Length, 2);
            Array.Copy(
                EndianBitConverter.ToByte((UInt16)RecordClass, false),
                0, QuestionBytes, this.EncodedName.Length + 2, 2);

            return QuestionBytes;
        }

        // Public Methods

        public override String ToString()
        {
            return String.Format("{0} {1} {2}",
                this.Name,
                this.RecordClass.ToString(),
                this.RecordType.ToString());
        }
    }
}
