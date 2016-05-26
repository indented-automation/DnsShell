using System;
using System.Management;
using System.Security;

namespace DnsShell.Management
{
    public class Cache
    {
        private ManagementObject wmiCache;

        public String ContainerName { get; private set; }
        public String DnsServerName { get; private set; }
        public String Name { get; private set; }
        public String ManagementPath { get; private set; }

        internal Cache(ManagementObject wmiCache)
        {
            this.wmiCache = wmiCache;

            this.ContainerName = (String)wmiCache.Properties["ContainerName"].Value;
            this.DnsServerName = (String)wmiCache.Properties["DnsServerName"].Value;
            this.Name = (String)wmiCache.Properties["Name"].Value;

            this.ManagementPath = wmiCache.Path.Path;
        }

        internal String ClearCache()
        {
            return (String)this.wmiCache.InvokeMethod("ClearCache", new object[] { });
        }

        internal String GetDistinguishedName()
        {
            return (String)this.wmiCache.InvokeMethod("GetDistinguishedName", new object[] { });
        }
    }
}
