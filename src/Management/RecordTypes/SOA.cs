using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class SOA : ResourceRecord
    {
        public String TargetName { get; private set; }
        public String ResponsiblePerson { get; private set; }
        public UInt32 Serial { get; private set; }
        public UInt32 Refresh { get; private set; }
        public UInt32 Retry { get; private set; }
        public UInt32 Expire { get; private set; }
        public UInt32 MinimumTTL { get; private set; }

        internal SOA(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.SOA;

            Serial = (UInt32)wmiRecord.Properties["SerialNumber"].Value;
            TargetName = (String)wmiRecord.Properties["PrimaryServer"].Value;
            ResponsiblePerson = (String)wmiRecord.Properties["ResponsibleParty"].Value;
            Refresh = (UInt32)wmiRecord.Properties["RefreshInterval"].Value;
            Retry = (UInt32)wmiRecord.Properties["RetryDelay"].Value;
            Expire = (UInt32)wmiRecord.Properties["ExpireLimit"].Value;
            MinimumTTL = (UInt32)wmiRecord.Properties["MinimumTTL"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt32 Serial = UInt32.MaxValue,
            String TargetName = "",
            String ResponsiblePerson = "",
            UInt32 Refresh = UInt32.MaxValue,
            UInt32 Retry = UInt32.MaxValue,
            UInt32 Expire = UInt32.MaxValue,
            UInt32 MinimumTTL = UInt32.MaxValue)
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (TargetName != String.Empty & TargetName != this.TargetName)
            {
                inParams["PrimaryServer"] = TargetName;
            }
            if (ResponsiblePerson != String.Empty & ResponsiblePerson != this.ResponsiblePerson)
            {
                inParams["ResponsibleParty"] = ResponsiblePerson;
            }
            if (Refresh != UInt32.MaxValue & Refresh != this.Refresh)
            {
                inParams["RefreshInterval"] = Refresh;
            }
            if (Retry != UInt32.MaxValue & Retry != this.Retry)
            {
                inParams["RetryDelay"] = Retry;
            }
            if (Expire != UInt32.MaxValue & Expire != this.Expire)
            {
                inParams["ExpireLimit"] = Expire;
            }
            if (MinimumTTL != UInt32.MaxValue & MinimumTTL != this.MinimumTTL)
            {
                inParams["MinimumTTL"] = MinimumTTL;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
