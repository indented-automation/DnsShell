using System;
using System.Collections.Generic;
using System.Text;

namespace DnsShell.Resolver
{
    public class NULL : ResourceRecord
    {
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                  <anything>                   /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public Byte[] Data;

        internal NULL(EndianBinaryReader Reader, UInt16 RDataLength)
        {
            base.UpdateProperties(Reader);

            Data = new Byte[RDataLength];
            for (Int32 i = 0; i < RDataLength; i++)
            {
                Data[i] = Reader.ReadByte();
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
