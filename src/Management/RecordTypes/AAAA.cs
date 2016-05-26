using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class AAAA : ResourceRecord
    {
        public IPAddress Address { get; private set; }

        internal AAAA(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.AAAA;
            this.Address = IPAddress.Parse((String)wmiRecord.Properties["IPv6Address"].Value);
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String Address = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Address != String.Empty & Address != this.Address.ToString())
            {
                inParams["IPv6Address"] = Address;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
