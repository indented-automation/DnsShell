using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class AFSDB : ResourceRecord
    {
        public UInt16 SubType { get; private set; }
        public String SubTypeName { get; private set; }
        public String TargetName { get; private set; }

        internal AFSDB(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.AFSDB;
            this.SubType = (UInt16)wmiRecord.Properties["SubType"].Value;
            this.TargetName = (String)wmiRecord.Properties["ServerName"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt16 SubType = UInt16.MaxValue,
            String TargetName = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (SubType != UInt16.MaxValue & SubType != this.SubType) 
            {
                inParams["SubType"] = SubType;
            }
            if (TargetName != String.Empty & TargetName != this.TargetName) 
            {
                inParams["ServerName"] = TargetName;
            }
            
            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
