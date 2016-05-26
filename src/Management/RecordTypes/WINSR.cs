using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class WINSR : ResourceRecord
    {
        public WINSMappingFlag MappingFlag { get; private set; }
        public UInt32 LookupTimeout { get; private set; }
        public UInt32 CacheTimeout { get; private set; }
        public String ResultDomain { get; private set; }

        internal WINSR(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.WINSR;
            this.MappingFlag = (WINSMappingFlag)wmiRecord.Properties["MappingFlag"].Value;
            this.LookupTimeout = (UInt32)wmiRecord.Properties["LookupTimeout"].Value;
            this.CacheTimeout = (UInt32)wmiRecord.Properties["CacheTimeout"].Value;
            this.ResultDomain = (String)wmiRecord.Properties["ResultDomain"].Value;

            this.RecordData = String.Format("L{0} C{1} ( {2} )", this.LookupTimeout, this.CacheTimeout, this.ResultDomain);
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            WINSMappingFlag MappingFlag = WINSMappingFlag.Replication,
            UInt32 LookupTimeout = UInt32.MaxValue,
            UInt32 CacheTimeout = UInt32.MaxValue,
            String ResultDomain = "")
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
            if (ResultDomain != String.Empty & ResultDomain != this.ResultDomain)
            {
                inParams["ResultDomain"] = ResultDomain;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
