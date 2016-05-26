using System;
using System.Text;
using System.IO;

namespace DnsShell.Resolver
{
    // DnsShell.Resolver.PacketReader - Contains methods to parse the Reply section from a DNS Packet.
    //
    // Constructors:
    //   Public: PacketReader(byte[] Response)
    //
    // Methods:
    //   Internal: DecodeName()

    public class PacketReader
    {
        //    +---------------------+
        //    |        Header       |
        //    +---------------------+
        //    |       Question      | the question for the name server
        //    +---------------------+
        //    |        Answer       | RRs answering the question
        //    +---------------------+
        //    |      Authority      | RRs pointing toward an authority
        //    +---------------------+
        //    |      Additional     | RRs holding additional information
        //    +---------------------+

        public Header Header;
        public Question[] Question;
        public ResourceRecord[] Answer;
        public ResourceRecord[] Authority;
        public ResourceRecord[] Additional;
        public Byte[] Bytes;

        private EndianBinaryReader Reader;

        // Constructors

        public PacketReader(Byte[] Response)
        {
            // Load the stream into the Binary Reader
            MemoryStream ResponseStream = new MemoryStream(Response);
            this.Reader = new EndianBinaryReader(ResponseStream);

            this.Bytes = Response;
            this.Header = new Header(this.Reader);

            this.Question = new Question[this.Header.QDCount];
            for (Int32 i = 0; i < this.Header.QDCount; i++)
            {
                this.Question[i] = new Question(Reader);
            }
            this.Answer = new ResourceRecord[this.Header.ANCount];
            for (Int32 i = 0; i < this.Header.ANCount; i++)
            {
                this.Answer[i] = new ResourceRecord(Reader);
            }
            this.Authority = new ResourceRecord[this.Header.NSCount];
            for (Int32 i = 0; i < this.Header.NSCount; i++)
            {
                this.Authority[i] = new ResourceRecord(Reader);
            }
            this.Additional = new ResourceRecord[this.Header.ARCount];
            for (Int32 i = 0; i < this.Header.ARCount; i++)
            {
                this.Additional[i] = new ResourceRecord(Reader);
            }
        }

        // Internal Methods

        public static String DecodeName(EndianBinaryReader Reader)
        {
            StringBuilder DecodedName = new StringBuilder();
            Int64 CompressionStart = 0;
            while (Reader.PeekChar() != 0)
            {
                Byte Length = Reader.ReadByte();
                if ((Length & (Byte)Compression.Enabled) == (Byte)Compression.Enabled)
                {
                    if (CompressionStart == 0) { CompressionStart = Reader.BaseStream.Position; }

                    UInt16 Offset = (UInt16)(((Byte)(Length ^ (Byte)Compression.Enabled) << 8) | Reader.ReadByte());
                    Reader.BaseStream.Seek(Offset, 0);
                }
                else
                {
                    DecodedName.Append(Reader.ReadChars(Length));
                    DecodedName.Append('.');
                }
            }
            // Read off the terminating 0
            Reader.ReadByte();
            // Or reset the position after the first compression instance
            if (CompressionStart > 0) { Reader.BaseStream.Seek(CompressionStart + 1, 0); }

            return DecodedName.ToString();
        }
    }
}
