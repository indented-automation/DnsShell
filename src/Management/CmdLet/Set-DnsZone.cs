using System;
using System.Management;
using System.Management.Automation;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Set, "DnsZone", SupportsShouldProcess = true)]
    public class SetDnsZone : ManagementCmdlet
    {
        #region Parameter:Identity (String) :: Parameter Set <All>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Identity",
            ParameterSetName = "")]
        [ValidatePattern(@"^\\\\.*\\root\\MicrosoftDNS:MicrosoftDNS_")]
        public String Identity;
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Server",
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetScope(Server);
            this.SetManagementPath(Identity);
            ManagementObject wmiZone = this.Get();

            // public void SetAging(
            //    Boolean EnableAging,
            //    TimeSpan NewNoRefreshInterval,
            //    TimeSpan NewRefreshInterval)
            //{
            //    String Key = String.Format(@"{0}\{1}",
            //                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones",
            //                    this.ContainerName);

            //    // if (!EnableAging) { Registry.SetValue(Key, "Aging", 0); }
            //    // if (EnableAging) { Registry.SetValue(Key, "Aging", 1); }

            //    // if (NewNoRefreshInterval != null) { Registry.SetValue(Key, "NoRefreshInterval", NewNoRefreshInterval.Hours); }
            //    // if (NewRefreshInterval != null) { Registry.SetValue(Key, "RefreshInterval", NewRefreshInterval.Hours); }
            //}

            //public void SetDynamicUpdate(
            //    ZoneDynamicUpdate NewDynamicUpdate)
            //{
            //    String Key = String.Format(@"{0}\{1}",
            //                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\DNS Server\Zones",
            //                    this.ContainerName);

            //    // Registry.SetValue(Key, "AllowUpdate", NewDynamicUpdate);

            //}


        }
    }
}
