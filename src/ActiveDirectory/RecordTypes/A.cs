using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class A : ResourceRecord
    {
        public IPAddress IPAddress { get; internal set; }

        internal A() { }

        internal A(Object Entry, int Index = 0)
        {
            if (Entry is SearchResultEntry)
            {
                base.UpdateProperties((SearchResultEntry)Entry, Index);
            }
            else if (Entry is DirectoryEntry)
            {
                base.UpdateProperties((DirectoryEntry)Entry, Index);
            }

            this.IPAddress = base.RecordReader.ReadIPAddress();
            base.RecordData = this.IPAddress.ToString();
        }

        public Byte[] ToByte()
        {
            Byte[] RRBytes = base.RRToByte();
            RRBytes[0] = 4;  // Data length is static

            Byte[] NewRecordBytes = new Byte[RRBytes.Length + 4];
            Array.Copy(RRBytes, NewRecordBytes, RRBytes.Length);
            Array.Copy(this.IPAddress.GetAddressBytes(), 0, NewRecordBytes, RRBytes.Length, 4);

            return NewRecordBytes;
        }
    }
}
