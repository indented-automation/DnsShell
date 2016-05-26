using System;
using System.Collections;
using System.Security;
using System.Management;

namespace DnsShell.Management
{
    public class Zone
    {
        private ManagementObject wmiZone;

        public String ZoneName { get; private set; }
        public Boolean AutoCreated { get; private set; }
        public String DataFile { get; private set; }
        public String DnsServerName { get; private set; }

        // Zone Type

        public ZoneType ZoneType { get; private set; }
        public Boolean ADIntegrated { get; private set; }
        public Boolean Reverse { get; private set; }

        // Aging and Scavenging

        public Boolean Aging { get; private set; }
        public ZoneDynamicUpdate DynamicUpdate { get; private set; }
        public Object AvailForScavengeTime { get; private set; }
        public TimeSpan NoRefreshInterval { get; private set; }
        public TimeSpan RefreshInterval { get; private set; }
        public String[] ScavengeServers { get; private set; }

        // Secondary Zones

        public Object LastSuccessfulSoaCheck { get; private set; }
        public Object LastSuccessfulXfr { get; private set; }
        public String[] MasterServers { get; private set; }
        public String[] LocalMasterServers { get; private set; }

        // Conditional Forwarders

        public Boolean ForwarderUseRecursion { get; private set; }
        public UInt32 ForwarderTimeOut { get; private set; }

        // Zone Transfers

        public ZoneTransfer ZoneTransferSetting { get; private set; }
        public String[] SecondaryServers { get; private set; }
        public Notify NotifySetting { get; private set; }
        public String[] NotifyServers { get; private set; }

        // Zone State

        public Boolean Paused { get; private set; }
        public Boolean Shutdown { get; private set; }

        // WINS Properties

        public Boolean DisableWINSRecordReplication { get; private set; }
        public Boolean UseWins { get; private set; }

        public String Identity { get; private set; }
        public String ServerName { get; internal set; }

        //
        // Constructors
        //

        public Zone(ManagementObject wmiZone, String ServerName)
        {
            this.wmiZone = wmiZone;
            this.ServerName = ServerName;
            this.Identity = wmiZone.Path.Path;

            UpdateProperties(wmiZone);
        }

        //
        // Private Methods
        //

        private void UpdateProperties(ManagementObject wmiZone)
        {
            // Zone Name

            this.ZoneName = (String)wmiZone.Properties["Name"].Value;
            this.AutoCreated = (Boolean)wmiZone.Properties["AutoCreated"].Value;
            this.DataFile = (String)wmiZone.Properties["DataFile"].Value;
            this.DnsServerName = (String)wmiZone.Properties["DnsServerName"].Value;

            // Zone Type

            this.ZoneType = (ZoneType)(UInt32)wmiZone.Properties["ZoneType"].Value;
            this.ADIntegrated = (Boolean)wmiZone.Properties["DsIntegrated"].Value;
            this.Reverse = (Boolean)wmiZone.Properties["Reverse"].Value;

            // Aging and Scavenging

            this.Aging = (Boolean)wmiZone.Properties["Aging"].Value;
            this.DynamicUpdate = (ZoneDynamicUpdate)(UInt32)wmiZone.Properties["AllowUpdate"].Value;

            if ((UInt32)wmiZone.Properties["AvailForScavengeTime"].Value != 0)
            {
                this.AvailForScavengeTime = new DateTime(1601, 1, 1).AddHours(
                    (UInt32)wmiZone.Properties["AvailForScavengeTime"].Value);
            }

            this.NoRefreshInterval = new TimeSpan((Int32)(UInt32)wmiZone.Properties["NoRefreshInterval"].Value, 0, 0);
            this.RefreshInterval = new TimeSpan((Int32)(UInt32)wmiZone.Properties["RefreshInterval"].Value, 0, 0);
            this.ScavengeServers = (String[])wmiZone.Properties["ScavengeServers"].Value;

            // Secondary Zones

            if ((UInt32)wmiZone.Properties["LastSuccessfulSoaCheck"].Value != 0)
            {
                this.LastSuccessfulSoaCheck = new DateTime(1970, 01, 01).AddSeconds(
                   (UInt32)wmiZone.Properties["LastSuccessfulSoaCheck"].Value);
            }

            if ((UInt32)wmiZone.Properties["LastSuccessfulXfr"].Value != 0)
            {
                this.LastSuccessfulXfr = new DateTime(1970, 01, 01).AddSeconds(
                    (UInt32)wmiZone.Properties["LastSuccessfulXfr"].Value);
            }

            this.MasterServers = (String[])wmiZone.Properties["MasterServers"].Value;
            this.LocalMasterServers = (String[])wmiZone.Properties["LocalMasterServers"].Value;

            // Conditional Forwarders

            this.ForwarderUseRecursion = (Boolean)wmiZone.Properties["ForwarderSlave"].Value;
            this.ForwarderTimeOut = (UInt32)wmiZone.Properties["ForwarderTimeOut"].Value;

            // Zone Transfers

            this.ZoneTransferSetting = (ZoneTransfer)(UInt32)wmiZone.Properties["SecureSecondaries"].Value;
            this.SecondaryServers = (String[])wmiZone.Properties["SecondaryServers"].Value;
            this.NotifySetting = (Notify)(UInt32)wmiZone.Properties["Notify"].Value;
            this.NotifyServers = (String[])wmiZone.Properties["NotifyServers"].Value;

            // Zone State

            this.Paused = (Boolean)wmiZone.Properties["Paused"].Value;
            this.Shutdown = (Boolean)wmiZone.Properties["Shutdown"].Value;

            // WINS

            this.DisableWINSRecordReplication = false;
            if (wmiZone.Properties["DisableWINSRecordReplication"].Value != null)
            {
                this.DisableWINSRecordReplication = (Boolean)wmiZone.Properties["DisableWINSRecordReplication"].Value;
            }
            this.UseWins = (Boolean)wmiZone.Properties["UseWins"].Value;
        }

        //
        // Public Methods
        //

        // Wrapper for AgeAllRecords Method
        //
        // This method call will fail if:
        //   NodeName is not fully-qualified or @
        //   NodeName does not exist

        internal UInt32 AgeAllRecords(String NodeName, Boolean ApplyToSubtree)
        {
            if (NodeName != "@" | !NodeName.Contains(this.ZoneName))
            {
                return 1;
            }

            return (UInt32)this.wmiZone.InvokeMethod("AgeAllRecords", new object[] { NodeName, ApplyToSubtree });
        }

        // Wrapper for ChangeZoneType
        //
        // Error Codes for ChangeZoneType:
        // Argument Error Codes:
        // 1 - IP Address List is mandatory for Secondary, Stub and Forwarder
        // 2 - Cannot change Stub or Forwarder to Primary
        // 3 - Cannot convert Shutdown or Expired zone to Primary
        // 4 - Operation only valid for Standard Primary Zones
        // 5 - Secondary Zones cannot be DsIntegrated
        // 6 - Cannot convert Secondary, Stub or Forwarder to DsIntegrated

        internal UInt32 ChangeZoneType(
            ZoneType NewZoneType,
            Boolean NewDsIntegrated,
            String NewDataFile,
            String[] NewIPAddressList)
        {
            // Populate fields any null fields if possible

            if (this.DataFile == null)
            {
                NewDataFile = String.Format("{0}.dns", this.ZoneName);
            }
            else
            {
                NewDataFile = this.DataFile;
            }
            if (NewZoneType != ZoneType.Primary & NewIPAddressList[0] == String.Empty)
            {
                if (this.MasterServers != null)
                {
                    NewIPAddressList = this.MasterServers;
                }
                else if (this.LocalMasterServers != null)
                {
                    NewIPAddressList = this.LocalMasterServers;
                }
                else
                {
                    return 1;
                }
            }

            // Scenarios that will error when changing to Primary

            if (NewZoneType == ZoneType.Primary)
            {
                if (this.ZoneType == ZoneType.Stub | this.ZoneType == ZoneType.Forwarder)
                {
                    return 2;
                }
                if (this.ZoneType == ZoneType.Secondary & this.Shutdown)
                {
                    return 3;
                }
                if (this.ZoneType != ZoneType.Primary & NewDsIntegrated)
                {
                    return 4;
                }
            }

            // Scenarios that will error when changing to Secondary

            if (NewZoneType == ZoneType.Secondary & NewDsIntegrated)
            {
                return 5;
            }

            // Scenarios that will error when changing to Stub and Forwarder

            if (this.ZoneType != ZoneType.Primary & NewDsIntegrated & !this.ADIntegrated)
            {
                return 6;
            }

            ManagementBaseObject inParams = this.wmiZone.GetMethodParameters("ChangeZoneType");
            inParams["DataFileName"] = NewDataFile;
            inParams["DsIntegrated"] = NewDsIntegrated;
            inParams["IpAddr"] = NewIPAddressList;
            inParams["ZoneType"] = (UInt32)NewZoneType - 1;

            this.wmiZone.InvokeMethod("ChangeZoneType", inParams, null);

            return 0;
        }

        internal void ForceRefresh()
        {
            this.wmiZone.InvokeMethod("ForceRefresh", new object[] { });
        }

        public String GetDistinguishedName()
        {
            return (String)this.wmiZone.InvokeMethod("GetDistinguishedName", new object[] { });
        }

        internal void PauseZone()
        {
            this.wmiZone.InvokeMethod("PauseZone", new object[] { });
            this.Paused = true;
        }

        internal void ReloadZone()
        {
            this.wmiZone.InvokeMethod("ReloadZone", new object[] { });
        }

        internal UInt32 SetZoneTransfer(
            ZoneTransfer NewZoneTransferSetting,
            String[] NewSecondaryServers,
            Notify NewNotifySetting,
            String[] NewNotifyServers)
        {
            if (this.ZoneType != ZoneType.Primary) { return 1; }

            ManagementBaseObject Parameters = this.wmiZone.GetMethodParameters("ResetSecondaries");
            Parameters["SecureSecondaries"] = NewZoneTransferSetting;
            if (NewSecondaryServers[0] != String.Empty)
            {
                Parameters["SecondaryServers"] = NewSecondaryServers;
            }
            Parameters["Notify"] = NewNotifySetting;
            if (NewNotifyServers[0] != String.Empty)
            {
                Parameters["NotifyServers"] = NewNotifyServers;
            }

            this.wmiZone.InvokeMethod("ResetSecondaries", Parameters, null);
            return 0;
        }

        internal void ResumeZone()
        {
            this.wmiZone.InvokeMethod("ResumeZone", new object[] { });
            this.Paused = false;
        }

        internal UInt32 UpdateFromDS()
        {
            if (this.ADIntegrated == true)
            {
                this.wmiZone.InvokeMethod("UpdateFromDS", new object[] { });
                return 0;
            }
            else
            {
                return 1;
            }
        }

        internal void WriteBackZone()
        {
            this.wmiZone.InvokeMethod("WriteBackZone", new object[] { });
        }
    }
}
