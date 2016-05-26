using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class KEY : ResourceRecord
    {
        public UInt16 Flags { get; private set; }
        public UInt16 Protocol { get; private set; }
        public UInt16 Algorithm { get; private set; }
        public String PublicKey { get; private set; }

        internal KEY(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.KEY;
            this.Flags = (UInt16)wmiRecord.Properties["Flags"].Value;
            this.Protocol = (UInt16)wmiRecord.Properties["Protocol"].Value;
            this.Algorithm = (UInt16)wmiRecord.Properties["Algorithm"].Value;
            this.PublicKey = (String)wmiRecord.Properties["PublicKey"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt16 Flags = UInt16.MaxValue,
            UInt16 Protocol = UInt16.MaxValue,
            UInt16 Algorithm = UInt16.MaxValue,
            String PublicKey = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Flags != UInt16.MaxValue & Flags != this.Flags) 
            {
                inParams["Flags"] = Flags;
            }
            if (Protocol != UInt16.MaxValue & Protocol != this.Protocol) 
            {
                inParams["Protocol"] = Protocol;
            }
            if (Algorithm != UInt16.MaxValue & Algorithm != this.Algorithm) 
            {
                inParams["Algorithm"] = Algorithm;
            }
            if (PublicKey != String.Empty & PublicKey != this.PublicKey) 
            {
                inParams["PublicKey"] = PublicKey;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
