using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnsShell.Resolver
{
    class PacketWriter
    {
        public static Byte[] EncodeName(string Name)
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
