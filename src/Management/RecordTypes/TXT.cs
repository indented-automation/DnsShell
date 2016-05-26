using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class TXT : ResourceRecord
    {
        public String Text { get; private set; }

        internal TXT(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.TXT;
            this.Text = (String)wmiRecord.Properties["DescriptiveText"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            String Text = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (Text != String.Empty & Text != this.Text)
            {
                inParams["DescriptiveText"] = Text;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
