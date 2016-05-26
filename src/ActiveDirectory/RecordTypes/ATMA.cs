using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Text;

namespace DnsShell.ActiveDirectory
{
    public class ATMA : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |                    SUBTYPE                    |
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                    HOSTNAME                   /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public ATMAFormat Format;
        public String ATMAddress;

        public ATMA(SearchResultEntry Entry, int Index = 0)
        {
            base.UpdateProperties(Entry, Index);
            this.Format = (ATMAFormat)base.RecordReader.ReadByte();

            StringBuilder ATMAddressTemp = new StringBuilder();
            if (Format == ATMAFormat.AESA)
            {
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    ATMAddressTemp.Append(base.RecordReader.ReadChar());
                }
            }
            else if (Format == ATMAFormat.E164)
            {
                ATMAddressTemp.Append("+");
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    if (i == 3 | i == 6) { ATMAddressTemp.Append("."); }
                    ATMAddressTemp.Append(base.RecordReader.ReadChar());
                }
            }
            else if (Format == ATMAFormat.NSAP)
            {
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    if (i == 1 | i == 3 | i == 13 | i == 19) { ATMAddressTemp.Append("."); }
                    ATMAddressTemp.Append(String.Format("{0:X2}", base.RecordReader.ReadByte()));
                }
            }
            ATMAddress = ATMAddressTemp.ToString();

            this.RecordData = this.ATMAddress;
        }
    }
}
