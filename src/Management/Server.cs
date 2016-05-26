using System;
using System.Collections;
using System.Management;

namespace DnsShell.Management
{
    public class Server
    {
        private ManagementObject wmiServer;

        // Server Name and Version

        public String ServerName { get; private set; }
        public String Version { get; private set; }

        // Settings

        public BootMethod BootMethod { get; private set; }
        public Boolean BindSecondaries { get; private set; }
        public Boolean DisableAutoReverseZones { get; private set; }

        // Response Controls

        public Boolean LocalNetPriority { get; private set; }
        public Boolean WriteAuthorityNS { get; private set; }
        public Boolean RoundRobin { get; private set; }
        public UInt32 AddressAnswerLimit { get; private set; }
        public UInt32 MaxCacheTTL { get; private set; }
        public UInt32 MaxNegativeCacheTTL { get; private set; }
        public Boolean ForwardDelegations { get; private set; }

        // Cache

        public Boolean AutoCacheUpdate { get; private set; }  // What does this actually do?
        public Boolean SecureResponses { get; private set; }
        public UInt32 CacheLockingPercent { get; internal set; }

        // Recursion

        public UInt32 RecursionRetry { get; private set; }
        public UInt32 RecursionTimeout { get; private set; }
        public Boolean NoRecursion { get; private set; }

        // Zone Load and update options

        public NameCheckFlag NameCheckFlag { get; private set; }
        public Boolean AllowCNAMEAtNS { get; internal set; }
        public AutoConfigZones AutoConfigFileZones { get; private set; }
        public Boolean StrictFileParsing { get; private set; }
        public Boolean LooseWildcarding { get; private set; }
        public Boolean DeleteOutsideGlue { get; internal set; }

        // Global Query Block List

        public Boolean EnableGlobalQueryBlockList { get; internal set; }
        public String[] GlobalQueryBlockList { get; internal set; }

        // Global Names

        public Boolean EnableGlobalNamesSupport { get; internal set; }
        public Boolean GlobalNamesQueryOrder { get; internal set; }
        public Boolean GlobalNamesBlockUpdates { get; internal set; }

        // Forwarders

        public String[] Forwarders { get; private set; }
        public UInt32 ForwardingTimeout { get; private set; }
        public Boolean IsSlave { get; private set; }

        // Aging and Scavenging

        public Boolean DefaultAgingState { get; private set; }
        public TimeSpan ScavengingInterval { get; private set; }
        public UInt32 DefaultRefreshInterval { get; private set; }
        public UInt32 DefaultNoRefreshInterval { get; private set; }

        // Dynamic Update

        public ServerDynamicUpdate AllowUpdate { get; private set; }
        public UInt32 UpdateOptions { get; private set; }

        // Active Directory

        public Boolean EnableDirectoryPartitions { get; private set; }
        public Boolean DsAvailable { get; private set; }
        public UInt32 DsPollingInterval { get; private set; }
        public UInt32 DsTombstoneInterval { get; private set; }

        // Logging

        public EventLogLevel EventLogLevel { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public String LogFilePath { get; private set; }
        public UInt32 LogFileMaxSize { get; private set; }
        public String[] LogIPFilterList { get; private set; }

        // Network Settings

        public String[] ServerAddresses { get; private set; }
        public String[] ListenAddresses { get; private set; }
        public UInt32 SendPort { get; private set; }
        // public Boolean DisjointNets { get; private set; } - The server is supposed to ignore this value
        public String[] RpcProtocol { get; private set; }
        public UInt32 SocketPoolSize { get; internal set; }
        public String[] SocketPoolExcludedPortRanges { get; internal set; }
        public UInt32 XfrConnectTimeout { get; private set; }

        // EDNS

        public Boolean EnableEDnsProbes { get; private set; }
        public UInt32 EDnsCacheTimeout { get; private set; }

        // DNSSEC

        public DnsSecMode EnableDnsSec { get; private set; }

        // Identity

        public String ManagementPath { get; private set; }

        //
        // Constructors
        //

        public Server(ManagementObject wmiServer)
        {
            this.wmiServer = wmiServer;
            this.ManagementPath = wmiServer.Path.Path;

            UpdateWmiProperties(wmiServer);
        }

        //
        // Private Methods
        //

        private void UpdateWmiProperties(ManagementObject wmiServer)
        {
            // Server Name and Version

            this.ServerName = (String)wmiServer.Properties["Name"].Value;
            UInt32 TempVersion  = (UInt32)wmiServer.Properties["Version"].Value;
            UInt16 ServicePackVersion = (UInt16)(TempVersion >> 16);
            Byte MinorVersion = (Byte)((TempVersion & (UInt32)UInt16.MaxValue) >> 8);
            Byte MajorVersion = (Byte)((TempVersion & (UInt32)UInt16.MaxValue) & Byte.MaxValue);
            this.Version = String.Format("{0}.{1}.{2}", MajorVersion, MinorVersion, ServicePackVersion);

            // Settings

            this.BootMethod = (BootMethod)(UInt32)wmiServer.Properties["BootMethod"].Value;
            this.NameCheckFlag = (NameCheckFlag)(UInt32)wmiServer.Properties["NameCheckFlag"].Value;
            this.AddressAnswerLimit = (UInt32)wmiServer.Properties["AddressAnswerLimit"].Value;
            this.RecursionRetry = (UInt32)wmiServer.Properties["RecursionRetry"].Value;
            this.RecursionTimeout = (UInt32)wmiServer.Properties["RecursionTimeout"].Value;
            this.MaxCacheTTL = (UInt32)wmiServer.Properties["MaxCacheTTL"].Value;
            this.MaxNegativeCacheTTL = (UInt32)wmiServer.Properties["MaxNegativeCacheTTL"].Value;
            this.StrictFileParsing = (Boolean)wmiServer.Properties["StrictFileParsing"].Value;
            this.LooseWildcarding = (Boolean)wmiServer.Properties["LooseWildcarding"].Value;
            this.BindSecondaries = (Boolean)wmiServer.Properties["BindSecondaries"].Value;
            this.DisableAutoReverseZones = (Boolean)wmiServer.Properties["DisableAutoReverseZones"].Value;
            this.AutoCacheUpdate = (Boolean)wmiServer.Properties["AutoCacheUpdate"].Value;
            this.NoRecursion = (Boolean)wmiServer.Properties["NoRecursion"].Value;
            this.RoundRobin = (Boolean)wmiServer.Properties["RoundRobin"].Value;
            this.LocalNetPriority = (Boolean)wmiServer.Properties["LocalNetPriority"].Value;
            this.WriteAuthorityNS = (Boolean)wmiServer.Properties["WriteAuthorityNS"].Value;
            this.ForwardDelegations = false;
            if ((UInt32)wmiServer.Properties["ForwardDelegations"].Value != 0)
            {
                this.ForwardDelegations = true;
            }
            this.SecureResponses = (Boolean)wmiServer.Properties["SecureResponses"].Value;
            this.AutoConfigFileZones = (AutoConfigZones)(UInt32)wmiServer.Properties["AutoConfigFileZones"].Value;
            this.XfrConnectTimeout = (UInt32)wmiServer.Properties["XfrConnectTimeout"].Value;

            // Forwarders

            this.Forwarders = (String[])wmiServer.Properties["Forwarders"].Value;
            this.ForwardingTimeout = (UInt32)wmiServer.Properties["ForwardingTimeout"].Value;
            this.IsSlave = (Boolean)wmiServer.Properties["IsSlave"].Value;
            
            // Aging and Scavenging

            this.ScavengingInterval = new TimeSpan((Int32)(UInt32)wmiServer.Properties["ScavengingInterval"].Value, 0, 0);
            this.DefaultAgingState = (Boolean)wmiServer.Properties["DefaultAgingState"].Value;
            this.DefaultRefreshInterval = (UInt32)wmiServer.Properties["DefaultRefreshInterval"].Value;
            this.DefaultNoRefreshInterval = (UInt32)wmiServer.Properties["DefaultNoRefreshInterval"].Value;

            // Dynamic Update

            this.AllowUpdate = (ServerDynamicUpdate)(UInt32)wmiServer.Properties["AllowUpdate"].Value;
            this.UpdateOptions = (UInt32)wmiServer.Properties["UpdateOptions"].Value;

            // Active Directory

            this.EnableDirectoryPartitions = (Boolean)wmiServer.Properties["EnableDirectoryPartitions"].Value;
            this.DsAvailable = (Boolean)wmiServer.Properties["DsAvailable"].Value;
            this.DsPollingInterval = (UInt32)wmiServer.Properties["DsPollingInterval"].Value;
            this.DsTombstoneInterval = (UInt32)wmiServer.Properties["DsTombstoneInterval"].Value;

            // Logging

            this.EventLogLevel = (EventLogLevel)(UInt32)wmiServer.Properties["EventLogLevel"].Value;
            this.LogLevel = (LogLevel)(UInt32)wmiServer.Properties["LogLevel"].Value;
            this.LogFilePath = (String)wmiServer.Properties["LogFilePath"].Value;
            this.LogFileMaxSize = (UInt32)wmiServer.Properties["LogFileMaxSize"].Value;
            this.LogIPFilterList = (String[])wmiServer.Properties["LogIPFilterList"].Value;

            // Network Settings

            this.ServerAddresses = (String[])wmiServer.Properties["ServerAddresses"].Value;
            this.ListenAddresses = (String[])wmiServer.Properties["ListenAddresses"].Value;
            if (wmiServer.Properties["ListenAddresses"].Value == null)
            {
                this.ListenAddresses = this.ServerAddresses;
            }
            // this.DisjointNets = (Boolean)wmiServer.Properties["DisjointNets"].Value;
            this.SendPort = (UInt32)wmiServer.Properties["SendPort"].Value;
            this.EnableEDnsProbes = (Boolean)wmiServer.Properties["EnableEDnsProbes"].Value;
            this.EDnsCacheTimeout = (UInt32)wmiServer.Properties["EDnsCacheTimeout"].Value;
            this.EnableDnsSec = (DnsSecMode)(UInt32)wmiServer.Properties["EnableDnsSec"].Value;

            Int32 RpcProtocolValue = (Int32)wmiServer.Properties["RpcProtocol"].Value;
            ArrayList RpcProtocolFlags = new ArrayList();
            foreach (String RpcFlag in Enum.GetNames(typeof(RpcProtocol)))
            {
                Int32 Value = (Int32)Enum.Parse(typeof(RpcProtocol), RpcFlag);
                if (((RpcProtocolValue & Value) == Value) & RpcFlag != "None")
                {
                    RpcProtocolFlags.Add(RpcFlag);
                }
            }
            if (RpcProtocolFlags.Count == 0)
            {
                RpcProtocolFlags.Add("None");
            }
            this.RpcProtocol = (String[])RpcProtocolFlags.ToArray(typeof(String));
        }

        //
        // Public Methods
        //

        public UInt32 StartService()
        {
            return (UInt32)this.wmiServer.InvokeMethod("StartService", new object[] { });
        }

        public UInt32 StopService()
        {
            return (UInt32)this.wmiServer.InvokeMethod("StopService", new object[] { });
        }

        public UInt32 StartScavenging()
        {
            return (UInt32)this.wmiServer.InvokeMethod("StartScavenging", new object[] { });
        }

        internal String GetDistinguishedName()
        {
            return (String)this.wmiServer.InvokeMethod("GetDistinguishedName", new object[] { });
        }
    }
}
