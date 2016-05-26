using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class HINFO : ResourceRecord
    {
        public String CPU { get; private set; }
        public String OS { get; private set; }

        internal HINFO(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.HINFO;

            this.CPU = (String)wmiRecord.Properties["CPU"].Value;
            this.OS = (String)wmiRecord.Properties["OS"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String CPU = "",
            String OS = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            } 
            if (CPU != String.Empty & CPU != this.CPU)
            {
                inParams["CPU"] = CPU;
            }
            if (OS != String.Empty & OS != this.OS) 
            {
                inParams["OS"] = OS;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
