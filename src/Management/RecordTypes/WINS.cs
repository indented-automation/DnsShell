using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class WINS : ResourceRecord
    {
        public WINSMappingFlag MappingFlag { get; private set; }
        public UInt32 LookupTimeout { get; private set; }
        public UInt32 CacheTimeout { get; private set; }
        public String WinsServers { get; private set; }

        internal WINS(ManagementObject wmiRecord, String ServerName)
        {
            base.UpdateProperties(wmiRecord);
            base.ServerName = ServerName;
            base.RecordType = RecordType.WINS;
            
            MappingFlag = (WINSMappingFlag)(UInt32)wmiRecord.Properties["MappingFlag"].Value;
            LookupTimeout = (UInt32)wmiRecord.Properties["LookupTimeout"].Value;
            CacheTimeout = (UInt32)wmiRecord.Properties["CacheTimeout"].Value;
            WinsServers = ((String)wmiRecord.Properties["WinsServers"].Value).Replace("\"", "").Trim();

            base.RecordData = String.Format("L{0} C{1} ( {2} )", this.LookupTimeout, this.CacheTimeout, this.WinsServers);
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            WINSMappingFlag MappingFlag = WINSMappingFlag.Replication,
            UInt32 LookupTimeout = UInt32.MaxValue,
            UInt32 CacheTimeout = UInt32.MaxValue,
            String WinsServers = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (MappingFlag != this.MappingFlag)
            {
                inParams["MappingFlag"] = (UInt32)MappingFlag;
            }
            if (LookupTimeout != UInt32.MaxValue & LookupTimeout != this.LookupTimeout)
            {
                inParams["LookupTimeout"] = LookupTimeout;
            }
            if (CacheTimeout != UInt32.MaxValue & CacheTimeout != this.CacheTimeout)
            {
                inParams["CacheTimeout"] = CacheTimeout;
            }
            if (WinsServers != String.Empty & WinsServers != this.WinsServers)
            {
                inParams["WinsServers"] = WinsServers;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
