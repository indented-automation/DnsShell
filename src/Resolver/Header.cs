using System;
using System.Collections;
using System.Text;
using System.IO;
using DnsShell;

namespace DnsShell.Resolver
{
    // DnsShell.Resolver.Query - Contains methods to build and parse a DNS packet header.
    //
    // Constructors:
    //   Public: Header()
    //   Public: Header(bool RD, ushort QDCount)
    //   Public: Header(Reply Response)
    //
    // Methods:
    //   Protected: ReadFlags(byte Flags1, byte Flags2)
    //   Public: ToByteArray()
    //   Public Override: ToString()

    public class Header
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                      ID                       |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    QDCOUNT                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    ANCOUNT                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    NSCOUNT                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    ARCOUNT                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 ID { get; private set; }
        public Boolean IsResponse { get; private set; } // QR
        public OpCode OpCode { get; private set; }
        public Boolean RecursionDesired = true;
        public Flag Flags { get; private set; }
        public RCode RCode { get; private set; }
        public UInt16 QuestionCount = 1;
        public UInt16 AnswerCount { get; private set; }
        public UInt16 AuthorityCount { get; internal set; }
        public UInt16 AdditionalCount { get; internal set; }

        // Constructors

        internal Header()
        {
            Random Rand = new Random();
            this.ID = (UInt16)Rand.Next(1, 32767);
        }

        internal Header(Boolean RecursionDesired, UInt16 QDCount)
        {
            this.RecursionDesired = RecursionDesired;
            this.QuestionCount = QDCount;

            Random Rand = new Random();
            this.ID = (UInt16)Rand.Next(1, 65536);
        }

        internal Header(EndianBinaryReader Response)
        {
            this.ID = Response.ReadUInt16(true);

            UInt16 Flags = Response.ReadUInt16(true);

            if ((Flags & (UInt16)QR.Response) == (UInt16)QR.Response)
            {
                this.IsResponse = true;
            }

            this.OpCode = (OpCode)((Flags & 30720) >> 11);
            this.Flags = (Flag)(Flags & 1968);
            this.RCode = (RCode)(Flags & 15);

            this.QuestionCount = Response.ReadUInt16(true);
            this.AnswerCount = Response.ReadUInt16(true);
            this.AuthorityCount = Response.ReadUInt16(true);
            this.AdditionalCount = Response.ReadUInt16(true);
        }

        // Internal Methods

        internal Byte[] ToByte()
        {
            Byte[] HeaderBytes = new Byte[12];

            Array.Copy(EndianBitConverter.ToByte(this.ID, false), HeaderBytes, 2);

            // Set Recursion desired, RD as a byte value.
            if (this.RecursionDesired == true) { HeaderBytes[2] |= (Byte)((UInt16)Flag.RD >> 8); };

            Array.Copy(EndianBitConverter.ToByte(this.QuestionCount, false), 0,
                HeaderBytes, 4, 2);

            // IXFR Support
            if (this.AuthorityCount > 0)
            {
                Array.Copy(EndianBitConverter.ToByte(this.AuthorityCount, false), 0, HeaderBytes, 8, 2);
            }
            if (this.AdditionalCount > 0)
            {
                Array.Copy(EndianBitConverter.ToByte(this.AdditionalCount, false), 0, HeaderBytes, 10, 2);
            }

            return HeaderBytes;
        }

        public override String ToString()
        {
            return String.Format("ID: {0} IsResponse: {1} OpCode: {2} RCode: {3} Flags: {4} " +
                "Query: {5} Answer: {6} Authority: {7} Additional: {8}",
                this.ID.ToString(),
                this.IsResponse.ToString().ToUpper(),
                this.OpCode.ToString().ToUpper(),
                this.RCode.ToString().ToUpper(),
                this.Flags.ToString(),
                this.QuestionCount,
                this.AnswerCount,
                this.AuthorityCount,
                this.AdditionalCount);
        }
    }
}
