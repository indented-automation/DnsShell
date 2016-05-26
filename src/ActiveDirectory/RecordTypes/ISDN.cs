using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace DnsShell.ActiveDirectory
{
    public class ISDN : ResourceRecord
    {
        public String ISDNAddress { get; private set; }
        public String SubAddress { get; private set; }

        private Byte ISDNAddressLength;
        private Byte SubAddressLength;

        public ISDN(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);

            this.ISDNAddressLength = base.RecordReader.ReadByte();
            this.ISDNAddress = new String(base.RecordReader.ReadChars(this.ISDNAddressLength));
            this.SubAddressLength = base.RecordReader.ReadByte();
            this.SubAddress = new String(base.RecordReader.ReadChars(this.SubAddressLength));

            base.RecordData = String.Format("\"{0}\" \"{1}\"", this.ISDNAddress, this.SubAddress);
        }
    }
}
