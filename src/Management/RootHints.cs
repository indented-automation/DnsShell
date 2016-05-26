using System;
using System.Management;

namespace DnsShell.Management
{
    class RootHints
    {
        private ManagementObject wmiRootHints;

        public String ContainerName { get; private set; }
        public String DnsServerName { get; private set; }
        public String Name { get; private set; }
        public String ManagementPath { get; private set; }

        internal RootHints(ManagementObject wmiRootHints)
        {
            this.wmiRootHints = wmiRootHints;

            this.ContainerName = (String)wmiRootHints.Properties["ContainerName"].Value;
            this.DnsServerName = (String)wmiRootHints.Properties["DnsServerName"].Value;
            this.Name = (String)wmiRootHints.Properties["Name"].Value;

            this.ManagementPath = wmiRootHints.Path.Path;
        }

        internal String WriteBackRootHintDatafile()
        {
            return (String)this.wmiRootHints.InvokeMethod("WriteBackRootHintDatafile", new object[] { });
        }

        internal String GetDistinguishedName()
        {
            return (String)this.wmiRootHints.InvokeMethod("GetDistinguishedName", new object[] { });
        }
    }
}
