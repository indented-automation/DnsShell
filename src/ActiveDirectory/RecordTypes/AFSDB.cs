using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace DnsShell.ActiveDirectory
{
    public class AFSDB : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    SUBTYPE                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    HOSTNAME                   /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public UInt16 SubType { get; private set; }
        public String SubTypeName { get; private set; }
        public String TargetName { get; private set; }

        internal AFSDB(SearchResultEntry Entry, int Index = 0)
        {
            this.UpdateProperties(Entry, Index);
            this.SubType = base.RecordReader.ReadUInt16(true);
            if (Enum.IsDefined(typeof(AFSDBSubType), SubType))
            {
                SubTypeName = (String)Enum.GetName(typeof(AFSDBSubType), this.SubType);
            }
            this.TargetName = base.ReadName();

            this.RecordData = String.Format("{0} {1}", SubType, TargetName);
        }
    }
}
