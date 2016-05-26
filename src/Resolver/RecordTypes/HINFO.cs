using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class HINFO : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                      CPU                      /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                       OS                      /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String CPU { get; private set; }
        public String OS { get; private set; }
        
        private Byte CPULength;
        private Byte OSLength;

        internal HINFO(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            this.CPULength = Reader.ReadByte();
            this.CPU = new String(Reader.ReadChars(CPULength));
            this.OSLength = Reader.ReadByte();
            this.OS = new String(Reader.ReadChars(OSLength));

            base.RecordData = String.Format("\"{0}\" \"{1}\"", CPU, OS);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
