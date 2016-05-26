using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class X25 : ResourceRecord
    {
        public String PSDNAddress { get; private set; }

        internal X25(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.X25;

            PSDNAddress = (String)wmiRecord.Properties["PSDNAddress"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String PSDNAddress = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (PSDNAddress != String.Empty & PSDNAddress != this.PSDNAddress)
            {
                inParams["PSDNAddress"] = PSDNAddress;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
