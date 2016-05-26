using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class SRV : ResourceRecord
    {
        public UInt16 Priority { get; private set; }
        public UInt16 Weight { get; private set; }
        public UInt16 Port { get; private set; }
        public String TargetName { get; private set; }

        internal SRV(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.SRV;
            this.Priority = (UInt16)wmiRecord.Properties["Priority"].Value;
            this.Weight = (UInt16)wmiRecord.Properties["Weight"].Value;
            this.Port = (UInt16)wmiRecord.Properties["Port"].Value;
            // SRVDomainName is only available with 2008. DomainName was used previously but is
            // used differently with 2008.
            // this.TargetName = (String)wmiRecord.Properties["SRVDomainName"].Value;
            String[] Temp = (base.RecordData).Split(' ');
            this.TargetName = Temp[Temp.Length - 1];
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt16 Priority = UInt16.MaxValue,
            UInt16 Weight = UInt16.MaxValue,
            UInt16 Port = UInt16.MaxValue,
            String TargetName = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Priority != UInt16.MaxValue & Priority != this.Priority)
            {
                inParams["Priority"] = Priority;
            }
            if (Weight != UInt16.MaxValue & Weight != this.Weight)
            {
                inParams["Weight"] = Weight;
            }
            if (Port != UInt16.MaxValue & Port != this.Port)
            {
                inParams["Port"] = Port;
            }
            if (TargetName != String.Empty & TargetName != this.TargetName)
            {
                inParams["SRVDomainName"] = TargetName;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
