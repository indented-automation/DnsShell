using System;
using System.Management;

namespace DnsShell.Management
{
    public class ResourceRecord
    {
        //
        // Static Methods
        //

        internal ManagementObject wmiResourceRecord;

        public String Name { get; internal set; }
        public UInt32 TTL { get; internal set; }
        public RecordClass RecordClass { get; internal set; }
        public RecordType RecordType { get; internal set; }
        public String RecordData { get; internal set; }
        public Object TimeStamp { get; internal set; }
        public String ZoneName { get; internal set; }
        public String DnsServerName { get; internal set; }

        public String Identity { get; internal set; }
        public String ServerName { get; internal set; }

        internal void UpdateProperties(ManagementObject wmiRecord)
        {
            this.wmiResourceRecord = wmiRecord;
            this.Identity = wmiRecord.Path.Path;

            this.Name = (String)wmiRecord.Properties["OwnerName"].Value;
            this.TTL = (UInt32)wmiRecord.Properties["TTL"].Value;
            this.RecordClass = (RecordClass)(UInt16)wmiRecord.Properties["RecordClass"].Value;
            this.RecordData = (String)wmiRecord.Properties["RecordData"].Value;

            if ((UInt32)wmiRecord.Properties["TimeStamp"].Value == 0)
            {
                this.TimeStamp = "Static";
            }
            else
            {
                this.TimeStamp = new DateTime(1601, 1, 1).AddHours(
                    (UInt32)wmiRecord.Properties["TimeStamp"].Value);
            }

            this.ZoneName = (String)wmiRecord.Properties["ContainerName"].Value;
            this.DnsServerName = (String)wmiRecord.Properties["DnsServerName"].Value;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4}", 
                this.Name, 
                this.TTL, 
                this.RecordClass, 
                this.RecordType, 
                this.RecordData);
        }
    }
}
