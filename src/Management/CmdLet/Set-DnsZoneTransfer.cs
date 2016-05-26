using System;
using System.Management;
using System.Management.Automation;
using System.Net;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Set, "DnsZoneTransfer")]
    public class SetDnsZoneTransfer : ManagementCmdlet
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
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        #region Parameter:ZoneTransfer (ZoneTransfer) :: Parameter Set <All>
        [Parameter(
            Mandatory = true,
            HelpMessage = "ZoneTransfer",
            ParameterSetName = "")]
        public ZoneTransfer ZoneTransfer;
        #endregion

        #region Parameter:SecondaryServers (String[]) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String[] SecondaryServers = new String[] { String.Empty };
        #endregion

        #region Parameter:Notify (Notify) :: Parameter Set <All>
        [Parameter(
            HelpMessage = "Notify",
            ParameterSetName = "")]
        public Notify Notify = Notify.None;
        #endregion

        #region Parameter:NotifyServers (String[]) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String[] NotifyServers = new String[] { String.Empty };
        #endregion

        #region Parameter:PassThru (Switch) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public SwitchParameter PassThru;
        #endregion

        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetScope(Server);
            this.SetManagementPath(Identity);

            if (ZoneTransfer == ZoneTransfer.List)
            {
                foreach (String SecondaryServer in SecondaryServers)
                {
                    IPAddress IPAddress;
                    if (!IPAddress.TryParse(SecondaryServer, out IPAddress))
                    {
                        ThrowTerminatingError(
                            new ErrorRecord(
                                new PSArgumentException("Invalid Secondary Server List"),
                                "InvalidArgument",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                }
            }

            if (Notify == Notify.List)
            {
                foreach (String NotifyServer in NotifyServers)
                {
                    IPAddress IPAddress;
                    if (!IPAddress.TryParse(NotifyServer, out IPAddress))
                    {
                        ThrowTerminatingError(
                            new ErrorRecord(
                                new PSArgumentException("Invalid Notify Server List"),
                                "InvalidArgument",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                }
            }

            ManagementObject wmiZone = this.Get();
            Zone DnsZone = new Zone(wmiZone, Server);

            if (DnsZone.ZoneType == ZoneType.Primary)
            {
                UInt32 ReturnCode = DnsZone.SetZoneTransfer(ZoneTransfer, SecondaryServers, Notify, NotifyServers);

                if (PassThru)
                {
                    wmiZone = this.Get();
                    WriteObject(new Zone(wmiZone, Server));
                }
            }
            else
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new PSInvalidOperationException("Only valid for Primary Zones"),
                        "InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        typeof(ManagementCmdlet)));
            }
        }
    }
}
