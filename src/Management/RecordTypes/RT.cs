using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class RT : ResourceRecord
    {
        public UInt16 Preference { get; private set; }
        public String TargetName { get; private set; }

        internal RT(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.RT;
            this.Preference = (UInt16)wmiRecord.Properties["Preference"].Value;
            this.TargetName = (String)wmiRecord.Properties["IntermediateHost"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt16 Preference = UInt16.MaxValue,
            String TargetName = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Preference != UInt16.MaxValue & Preference != this.Preference)
            {
                inParams["Preference"] = Preference;
            }
            if (TargetName != String.Empty & TargetName != this.TargetName)
            {
                inParams["IntermediateHost"] = TargetName;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
