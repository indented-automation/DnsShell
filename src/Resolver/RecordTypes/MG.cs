﻿using System;
using System.Text;
using System.Net;
using DnsShell.Resolver;
using System.IO;

namespace DnsShell.Resolver
{
    public class MG : ResourceRecord
    {
        //    EXPERIMENTAL
        //                                    1  1  1  1  1  1
        //      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    /                   MGMNAME                     /
        //    /                                               /
        //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        public String MailboxName;

        internal MG(EndianBinaryReader Reader)
        {
            base.UpdateProperties(Reader);

            MailboxName = DnsPacket.DecodeName(Reader);

            base.RecordData = MailboxName.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
