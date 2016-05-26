using System;
using System.Text;
using System.Net;
using System.IO;

namespace DnsShell.Resolver
{
    public class ATMA : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    |         FORMAT        |                       |
        //    +--+--+--+--+--+--+--+--+                       |
        //    /                   ATMADDRESS                  /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public ATMAFormat Format;
        public String ATMAddress;

        internal ATMA(EndianBinaryReader Reader, UInt16 RDataLength)
        {
            base.UpdateProperties(Reader);

            Format = (ATMAFormat)Reader.ReadByte();

            StringBuilder ATMAddressTemp = new StringBuilder();
            if (Format == ATMAFormat.AESA)
            {
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    ATMAddressTemp.Append(Reader.ReadChar());
                }
            }
            else if (Format == ATMAFormat.E164)
            {
                ATMAddressTemp.Append("+");
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    if (i == 3 | i == 6) { ATMAddressTemp.Append("."); }
                    ATMAddressTemp.Append(Reader.ReadChar());
                }
            }
            else if (Format == ATMAFormat.NSAP)
            {
                for (int i = 0; i < (RDataLength - 1); i++)
                {
                    if (i == 1 | i == 3 | i == 13 | i == 19) { ATMAddressTemp.Append("."); }
                    ATMAddressTemp.Append(String.Format("{0:X2}", Reader.ReadByte()));
                }
            }

            ATMAddress = ATMAddressTemp.ToString();

            base.RecordData = ATMAddress;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
