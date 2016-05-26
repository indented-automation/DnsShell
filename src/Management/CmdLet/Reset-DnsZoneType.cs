using System;
using System.Management;
using System.Management.Automation;
using System.Net;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Reset, "DnsZoneType")]
    public class ResetDnsZoneType : ManagementCmdlet
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

        #region Parameter:NewZoneType (ZoneType) :: Parameter Set <All>
        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "NewZoneType",
            ParameterSetName = "")]
        [Alias("NewType")]
        public ZoneType NewZoneType;
        #endregion

        #region Parameter:NewIPAddressList (String[]) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String[] NewIPAddressList = new String[] { String.Empty };
        #endregion

        #region Parameter:ADIntegrated (Boolean) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public Boolean ADIntegrated = false;
        #endregion

        #region Parameter:FileName (String) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String FileName;
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

            ManagementObject wmiZone = this.Get();
            Zone DnsZone = new Zone(wmiZone, Server);

            UInt32 ReturnCode = DnsZone.ChangeZoneType(NewZoneType, ADIntegrated, FileName, NewIPAddressList);
            switch (ReturnCode)
            {
                case 0:
                    // Blimey it worked.
                    if (PassThru)
                    {
                        wmiZone = this.Get();
                        WriteObject(new Zone(wmiZone, Server));
                    }
                    break;
                case 1:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSArgumentException("IP Address List is mandatory for Secondary, Stub and Forwarder"),
                            "InvalidArgument",
                            ErrorCategory.InvalidArgument,
                            typeof(ManagementCmdlet)));
                    break;
                case 2:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSNotSupportedException("Cannot change Stub or Forwarder to Primary"),
                            "NotSupported",
                            ErrorCategory.InvalidOperation,
                            typeof(ManagementCmdlet)));
                    break;
                case 3:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSNotSupportedException("Cannot convert Shutdown or Expired zone to Primary"),
                            "NotSupported",
                            ErrorCategory.InvalidOperation,
                            typeof(ManagementCmdlet)));
                    break;
                case 4:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSNotSupportedException("Operation only valid for Standard Primary Zones"),
                            "NotSupported",
                            ErrorCategory.InvalidOperation,
                            typeof(ManagementCmdlet)));
                    break;
                case 5:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSNotSupportedException("Secondary Zones cannot be DsIntegrated"),
                            "NotSupported",
                            ErrorCategory.InvalidOperation,
                            typeof(ManagementCmdlet)));
                    break;
                case 6:
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSNotSupportedException("Cannot convert Secondary, Stub or Forwarder to DsIntegrated"),
                            "NotSupported",
                            ErrorCategory.InvalidOperation,
                            typeof(ManagementCmdlet)));
                    break;
            }
        }
    }
}
