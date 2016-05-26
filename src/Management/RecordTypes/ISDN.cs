using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class ISDN : ResourceRecord
    {
        public String ISDNNumber { get; private set; }
        public String SubAddress { get; private set; }

        internal ISDN(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.ISDN;
            
            this.ISDNNumber = (String)wmiRecord.Properties["ISDNNumber"].Value;
            this.SubAddress = (String)wmiRecord.Properties["SubAddress"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String ISDNNumber = "",
            String SubAddress = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (ISDNNumber != String.Empty & ISDNNumber != this.ISDNNumber)
            {
                inParams["ISDNNumber"] = ISDNNumber; 
            }
            if (SubAddress != String.Empty & SubAddress != this.SubAddress) 
            {
                inParams["SubAddress"] = SubAddress;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
