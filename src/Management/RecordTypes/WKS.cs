using System;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace DnsShell.Management
{
    public class WKS : ResourceRecord
    {
        public IPAddress Address { get; private set; }
        public ProtocolType IPProtocol { get; private set; }
        public String Services { get; private set; }

        internal WKS(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.WKS;
            this.Address = IPAddress.Parse((String)wmiRecord.Properties["InternetAddress"].Value);
            this.IPProtocol = (ProtocolType)Enum.Parse(typeof(ProtocolType), (String)wmiRecord.Properties["IPProtocol"].Value, true);
            this.Services = (String)wmiRecord.Properties["Services"].Value;
        }
        
        internal String Modify(
            IPAddress Address,
            UInt32 TTL = UInt32.MaxValue,
            ProtocolType IPProtocol = ProtocolType.Unspecified,
            String Services = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Address != this.Address)
            {
                inParams["InternetAddress"] = Address;
            }
            if (IPProtocol != ProtocolType.Unspecified & IPProtocol != this.IPProtocol)
            {
                inParams["IPProtocol"] = IPProtocol;
            }
            if (Services != String.Empty & Services != this.Services)
            {
                inParams["Services"] = Services;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
