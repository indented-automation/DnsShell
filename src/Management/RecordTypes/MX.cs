using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class MX : ResourceRecord
    {
        public UInt16 Preference { get; private set; }
        public String TargetName { get; private set; }

        internal MX(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.MX;

            Preference = (UInt16)wmiRecord.Properties["Preference"].Value;
            TargetName = (String)wmiRecord.Properties["MailExchange"].Value;
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
                inParams["MailExchange"] = TargetName;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
