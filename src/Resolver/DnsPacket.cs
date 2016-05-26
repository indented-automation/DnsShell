using System;
using System.Collections;
using System.IO;
using System.Text;

namespace DnsShell.Resolver
{
    public class DnsPacket
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

        public Header Header { get; private set; }
        public Question[] Question { get; private set; }
        public String QuestionToString { get; private set; }
        public ResourceRecord[] Answer { get; private set; }
        public String AnswerToString { get; private set; }
        public ResourceRecord[] Authority { get; private set; }
        public String AuthorityToString { get; private set; }
        public ResourceRecord[] Additional { get; private set; }
        public String AdditionalToString { get; private set; }
        public EDns EDns { get; private set; }
        
        public String Server { get; internal set; }
        public Int32 Size { get; internal set; }
        public Double TimeTaken { get; internal set; }

        internal Byte[] Bytes;

        private EndianBinaryReader Reader;

        // Constructors

        internal DnsPacket(Byte[] Response)
        {
            // Load the stream into the Binary Reader
            MemoryStream ResponseStream = new MemoryStream(Response);
            this.Reader = new EndianBinaryReader(ResponseStream);

            this.Bytes = Response;
            this.Header = new Header(this.Reader);

            this.Question = new Question[this.Header.QuestionCount];
            for (Int32 i = 0; i < this.Header.QuestionCount; i++)
            {
                this.Question[i] = new Question(Reader);
            }
            this.QuestionToString = ConvertToString(this.Question);

            this.Answer = new ResourceRecord[this.Header.AnswerCount];
            for (Int32 i = 0; i < this.Header.AnswerCount; i++)
            {
                this.Answer[i] = this.ResourceRecord(Reader);
            }
            this.AnswerToString = ConvertToString(this.Answer);

            this.Authority = new ResourceRecord[this.Header.AuthorityCount];
            for (Int32 i = 0; i < this.Header.AuthorityCount; i++)
            {
                this.Authority[i] = this.ResourceRecord(Reader);
            }
            this.AuthorityToString = ConvertToString(this.Authority);

            ArrayList Temp = new ArrayList();
            for (Int32 i = 0; i < this.Header.AdditionalCount; i++)
            {
                ResourceRecord Record = this.ResourceRecord(Reader);
                if (Record is ResourceRecord)
                {
                    Temp.Add(Record);
                }
            }
            this.Additional = (ResourceRecord[])Temp.ToArray(typeof(ResourceRecord));
            this.AdditionalToString = ConvertToString(this.Additional);

            this.Size = Response.Length;
        }

        // Private Methods

        private ResourceRecord ResourceRecord(EndianBinaryReader Reader)
        {
            Int64 Position = Reader.BaseStream.Position;
            String Name = DnsPacket.DecodeName(Reader);
            RecordType RecordType = (RecordType)Reader.ReadUInt16(true);

            if (RecordType == RecordType.OPT)
            {
                Reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                this.EDns = new EDns(Reader);
                return null;
            }
            else
            {
                RecordClass RecordClass = (RecordClass)Reader.ReadUInt16(true);
                UInt32 TTL = Reader.ReadUInt32(true);
                UInt16 RDataLength = Reader.ReadUInt16(true);
                Reader.BaseStream.Seek(Position, SeekOrigin.Begin);

                Object RData = new Object();
                switch (RecordType)
                {
                    case RecordType.A: RData = new A(Reader); break;  // Tested
                    case RecordType.NS: RData = new NS(Reader); break;  // Tested
                    case RecordType.MD: RData = new MD(Reader); break;
                    case RecordType.MF: RData = new MF(Reader); break;
                    case RecordType.CNAME: RData = new CNAME(Reader); break;  // Tested
                    case RecordType.SOA: RData = new SOA(Reader); break;  // Tested
                    case RecordType.MB: RData = new MB(Reader); break; // Tested
                    case RecordType.MG: RData = new MG(Reader); break;
                    case RecordType.MR: RData = new MR(Reader); break;
                    case RecordType.NULL: RData = new NULL(Reader, RDataLength); break;
                    case RecordType.WKS: RData = new WKS(Reader, RDataLength); break;
                    case RecordType.PTR: RData = new PTR(Reader); break; // Tested
                    case RecordType.HINFO: RData = new HINFO(Reader); break; // Tested
                    case RecordType.MINFO: RData = new MINFO(Reader); break;
                    case RecordType.MX: RData = new MX(Reader); break; // Tested
                    case RecordType.TXT: RData = new TXT(Reader); break; // Tested
                    case RecordType.RP: RData = new RP(Reader); break;
                    case RecordType.AFSDB: RData = new AFSDB(Reader); break; // Tested
                    case RecordType.X25: RData = new X25(Reader); break;
                    case RecordType.ISDN: RData = new ISDN(Reader); break;
                    case RecordType.RT: RData = new RT(Reader); break;
                    case RecordType.NSAP: RData = new NSAP(Reader); break;
                    case RecordType.AAAA: RData = new AAAA(Reader); break; // Tested
                    case RecordType.SRV: RData = new SRV(Reader); break;  // Tested
                    case RecordType.ATMA: RData = new ATMA(Reader, RDataLength); break; // Tested
                    case RecordType.WINS: RData = new WINS(Reader); break; // Tested
                    case RecordType.WINSR: RData = new WINSR(Reader); break; // Tested
                    default: RData = Reader.ReadBytes(RDataLength); break;
                }

                return (ResourceRecord)RData;
            }
        }

        private String ConvertToString(Object[] DataSet)
        {
            if (DataSet.Length > 0)
            {
                String[] StringData = new String[DataSet.Length];

                for (int i = 0; i < DataSet.Length; i++)
                {
                    StringData[i] = DataSet[i].ToString();
                }
                return String.Join("\n", StringData);
            }
            return "";
        }

        // Internal Methods

        internal static String DecodeName(EndianBinaryReader Reader)
        {
            StringBuilder DecodedName = new StringBuilder();
            Int64 CompressionStart = 0;
            while (Reader.PeekByte() != 0)
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

        internal static Byte[] EncodeName(string Name)
        {
            Int32 Length = 1;
            if (Name.Length > 0)
            {
                Length = Name.Length + 2;
            }

            Byte[] EncodedName = new byte[Length];

            String[] Labels = Name.Split('.');
            Int32 i = 0;
            foreach (String Label in Labels)
            {
                EncodedName[i++] = (Byte)Label.Length;
                Byte[] LabelBytes = Encoding.ASCII.GetBytes(Label);
                Array.Copy(LabelBytes, 0, EncodedName, i, LabelBytes.Length);
                i += LabelBytes.Length;
            }

            return EncodedName;
        }
    }
}
