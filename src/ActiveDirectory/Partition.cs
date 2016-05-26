using System;
using System.DirectoryServices.Protocols;

namespace DnsShell.ActiveDirectory
{
    class Partition
    {
        public String DN { get; private set; }
        public String Type { get; private set; }
        public String[] ReplicaLocations { get; private set; }
        public Guid ObjectGuid { get; private set; }
        public DateTime WhenChanged { get; internal set; }
        public DateTime WhenCreated { get; internal set; }

        internal Partition(SearchResultEntry Entry)
        {
            this.DN = (String)Entry.Attributes["ncname"].GetValues(typeof(String))[0];

            Boolean HasServerList = false; 
            foreach (String AttributeName in Entry.Attributes.AttributeNames)
            {
                if (AttributeName == "msds-nc-replica-locations")
                {
                    HasServerList = true;
                }
                if (AttributeName == "netbiosname")
                {
                    this.Type = "Legacy";
                    this.DN = "CN=MicrosoftDNS,CN=System," + this.DN;
                }
            }

            if (this.DN.Contains("DC=DomainDnsZones,"))
            {
                this.Type = "Domain";
            }
            else if (this.DN.Contains("DC=ForestDnsZones,"))
            {
                this.Type = "Forest";
            }
            else if (this.Type != "Legacy")
            {
                this.Type = "Custom";
            }

            if (HasServerList)
            {
                this.ReplicaLocations = new String[Entry.Attributes["msds-nc-replica-locations"].Count];
                for (int i = 0; i < Entry.Attributes["msds-nc-replica-locations"].Count; i++)
                {
                    this.ReplicaLocations[i] = (String)Entry.Attributes["msds-nc-replica-locations"].GetValues(typeof(String))[i];
                }
            }

            this.ObjectGuid = new Guid((Byte[])Entry.Attributes["objectguid"].GetValues(typeof(Byte[]))[0]);

            this.WhenChanged = DateTime.ParseExact(
                (String)Entry.Attributes["whenchanged"].GetValues(typeof(String))[0],
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture);

            this.WhenCreated = DateTime.ParseExact(
                (String)Entry.Attributes["whencreated"].GetValues(typeof(String))[0],
                "yyyyMMddHHmmss.0Z",
                System.Globalization.CultureInfo.InvariantCulture).ToLocalTime();
        }
    }
}
