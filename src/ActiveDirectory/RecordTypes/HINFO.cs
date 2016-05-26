using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace DnsShell.ActiveDirectory
{
    public class HINFO : ResourceRecord
    {
        public String CPU { get; private set; }
        public String OS { get; private set; }

        private Byte CPULength;
        private Byte OSLength;

        public HINFO(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            this.CPULength = base.RecordReader.ReadByte();
            this.CPU = new String(base.RecordReader.ReadChars(this.CPULength));
            this.OSLength = base.RecordReader.ReadByte();
            this.OS = new String(base.RecordReader.ReadChars(this.OSLength));

            base.RecordData = String.Format("\"{0}\" \"{1}\"", this.CPU, this.OS);
        }
    }
}
