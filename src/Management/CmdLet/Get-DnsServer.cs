using System;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "DnsServer")]
    public class GetDnsServer : ManagementCmdlet
    {
        #region Parameter:Server (String[])
        [Parameter(
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public string[] Server = new string[] { "localhost" };
        #endregion

        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetManagementPath("MicrosoftDNS_Server");

            foreach (String ServerName in Server)
            {
                this.SetScope(ServerName);
                ManagementObjectCollection wmiResults = this.Search("");
                if (wmiResults != null)
                {
                    foreach (ManagementObject wmiServer in wmiResults)
                    {
                        Server ServerObject = new Server(wmiServer);

                        this.ConnectRegistry(ServerName);

                        // Populate values from the Registry

                        String RegKey = @"SYSTEM\CurrentControlSet\services\DNS\Parameters";

                        // AllowCNAMEAtNS

                        UInt32 AllowCNAMEAtNS = this.GetDWordValue(
                            RegKey,
                            "AllowCNAMEAtNS");
                        if (AllowCNAMEAtNS == 1)
                        {
                            ServerObject.AllowCNAMEAtNS = true;
                        }

                        // CacheLockingPercent

                        ServerObject.CacheLockingPercent = this.GetDWordValue(
                            RegKey,
                            "CacheLockingPercent");
                        if (ServerObject.CacheLockingPercent == UInt32.MaxValue)
                        {
                            ServerObject.CacheLockingPercent = 100;
                        }

                        // EnableGlobalQueryBlockList

                        UInt32 EnableGlobalQueryBlockList = this.GetDWordValue(
                            RegKey,
                            "EnableGlobalQueryBlockList");
                        if (EnableGlobalQueryBlockList == 1)
                        {
                            ServerObject.EnableGlobalQueryBlockList = true;
                        }

                        // GlobalQueryBlockList

                        ServerObject.GlobalQueryBlockList = this.GetMultiStringValue(
                            RegKey,
                            "GlobalQueryBlockList");

                        // EnableGlobalNamesSupport

                        UInt32 EnableGlobalNamesSupport = this.GetDWordValue(
                            RegKey,
                            "EnableGlobalNamesSupport");
                        if (EnableGlobalNamesSupport == 1)
                        {
                            ServerObject.EnableGlobalNamesSupport = true;
                        }

                        // GlobalNamesQueryOrder

                        UInt32 GlobalNamesQueryOrder = this.GetDWordValue(
                            RegKey,
                            "GlobalNamesQueryOrder");
                        if (GlobalNamesQueryOrder == 1)
                        {
                            ServerObject.GlobalNamesQueryOrder = true;
                        }

                        // GlobalNamesBlockUpdates

                        UInt32 GlobalNamesBlockUpdates = this.GetDWordValue(
                            RegKey,
                            "GlobalNamesBlockUpdates");
                        if (GlobalNamesBlockUpdates == 1)
                        {
                            ServerObject.GlobalNamesBlockUpdates = true;
                        }

                        // SocketPoolSize

                        ServerObject.SocketPoolSize = this.GetDWordValue(
                            RegKey,
                            "SocketPoolSize");
                        if (ServerObject.SocketPoolSize == UInt32.MaxValue | ServerObject.SocketPoolSize == 0)
                        {
                            // Default value for SocketPoolSize
                            ServerObject.SocketPoolSize = 2500;
                        }
                        
                        // SocketPoolExcludedPortRanges

                        ServerObject.SocketPoolExcludedPortRanges = this.GetMultiStringValue(
                            RegKey,
                            "SocketPoolExcludedPortRanges");

                        WriteObject(ServerObject);
                    }
                }
            }
        }
    }
}
