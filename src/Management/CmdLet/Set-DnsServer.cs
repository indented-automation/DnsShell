using System;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Set, "DnsServer")]
    public class SetDnsServer : Cmdlet
    {
        #region Parameter:BootMethod (BootMethod)
        [Parameter()]
        public BootMethod BootMethod;
        #endregion

        #region Parameter:NameCheckFlag (NameCheckFlag)
        [Parameter()]
        public NameCheckFlag NameCheckFlag;
        #endregion

        #region Parameter:AllowCNAMEAtNS (Boolean)
        [Parameter()]
        public Boolean AllowCNAMEAtNS;
        #endregion
        
        #region Parameter:AddressAnswerLimit (UInt32)
        [Parameter()]
        [ValidateRange(0, 28)]
        public UInt32 AddressAnswerLimit;
        #endregion

        #region Parameter:RecursionRetry (UInt32)
        [Parameter()]
        public UInt32 RecursionRetry;
        #endregion

        public UInt32 RecursionTimeout;
        public UInt32 MaxCacheTTL;
        public UInt32 MaxNegativeCacheTTL;

        // a composite value
        #region Parameter:AutoConfigFileZones (AutoConfigFileZones[])
        [Parameter()]
        public AutoConfigZones AutoConfigFileZones;
        #endregion

        public UInt32 XfrConnectTimeout;
        public Boolean ForwardDelegations;
        public UInt32 CacheLockingPercent;
        public Boolean StrictFileParsing;
        public Boolean LooseWildcarding;

        #region Parameter:BindSecondaries (Boolean)
        [Parameter()]
        public Boolean BindSecondaries;
        #endregion

        public Boolean DisableAutoReverseZones;

        #region Parameter:AutoCacheUpdate (Boolean)
        [Parameter()]
        public Boolean AutoCacheUpdate;
        #endregion

        public Boolean NoRecursion;
        public Boolean RoundRobin;
        public Boolean LocalNetPriority;
        public Boolean WriteAuthorityNS;
        public Boolean SecureResponses;
        public Boolean EnableGlobalQueryBlockList;
        public String[] GlobalQueryBlockList;
        public Boolean EnableGlobalNamesSupport;
        public Boolean GlobalNamesQueryOrder;
        public Boolean GlobalNamesBlockUpdates;

        // Forwarders

        public String[] Forwarders;
        public UInt32 ForwardingTimeout;

        // Aging and Scavenging

        #region Parameter:DefaultAgingState (Boolean)
        [Parameter()]
        public Boolean DefaultAgingState;
        #endregion

        #region Parameter:ScavengingInterval (TimeSpan)
        [Parameter()]
        public TimeSpan ScavengingInterval;
        #endregion

        #region Parameter:DefaultRefreshInterval (UInt32) - Should be TimeSpan?
        [Parameter()]
        public UInt32 DefaultRefreshInterval;
        #endregion

        #region Parameter:DefaultNoRefreshInterval (UInt32) - Should be TimeSpan?
        [Parameter()]
        public UInt32 DefaultNoRefreshInterval;
        #endregion

        // Dynamic Update

        #region Parameter:AllowUpdate (ServerDynamicUpdate)
        [Parameter()]
        public ServerDynamicUpdate AllowUpdate;
        #endregion

        public UInt32 UpdateOptions;

        // Active Directory

        public UInt32 DsPollingInterval;
        public UInt32 DsTombstoneInterval;

        // Logging

        public EventLogLevel EventLogLevel;
        public LogLevel LogLevel;
        public String LogFilePath;
        public UInt32 LogFileMaxSize;
        public String[] LogIPFilterList;

        // Network Settings

        public String[] ListenAddresses;
        public UInt32 SendPort;
        // public Boolean DisjointNets;
        public Boolean EnableEDnsProbes;
        public UInt32 EDnsCacheTimeout;
        public DnsSecMode EnableDnsSec;
        public UInt32 SocketPoolSize;
        public String[] SocketPoolExcludedPortRanges;

        #region Parameter:Server (String[])
        [Parameter(
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String[] Server = new string[] { "localhost" };
        #endregion
        
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }
    }
}
