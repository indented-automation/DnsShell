using System;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.New, "DnsZone", SupportsShouldProcess = true)]
    public class NewDnsZone : ManagementCmdlet
    {
        #region Parameter:ZoneName (String) :: Parameter Set <All>
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("Name", "ContainerName")]
        [ValidatePattern(@"^([A-Z0-9]|_[A-Z])(([\w\-]{0,61})[^_\-])?(\.([A-Z0-9]|_[A-Z])(([\w\-]{0,61})[^_\-.])?)*$")]
        public String ZoneName;
        #endregion

        #region Parameter:ZoneType (ZoneType) :: Parameter Set <All>
        [Parameter(
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("Type")]
        public ZoneType ZoneType;
        #endregion

        #region Parameter:ResponsiblePerson (String) :: Parameter Set <All>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        public String ResponsiblePerson = String.Empty;
        #endregion

        #region Parameter:FileName (String) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String FileName = String.Empty;
        #endregion

        #region Parameter:ADIntegrated (Switch) :: Parameter Set <All>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        public SwitchParameter ADIntegrated;
        #endregion

        #region Parameter:MasterServer (String[]) :: Parameter Set <All>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("MasterServers")]
        public String[] MasterServer = new String[] { String.Empty };
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        #region Parameter:PassThru (Switch) :: Parameter Set <All>
        [Parameter(
            HelpMessage = "PassThru",
            ParameterSetName = "")]
        public SwitchParameter PassThru;
        #endregion

        protected override void ProcessRecord()
        {
            base.SetOptions();
            base.SetScope(Server);
            base.SetManagementPath("MicrosoftDNS_Zone");

            ManagementClass WmiZoneClass = this.GetClass();
            String NewPath = String.Empty;

            ManagementBaseObject inParams = WmiZoneClass.GetMethodParameters("CreateZone");
            inParams["ZoneName"] = this.ZoneName;
            inParams["ZoneType"] = (UInt32)this.ZoneType - 1;

            if (ADIntegrated & this.ZoneType != ZoneType.Secondary)
            {
                inParams["DsIntegrated"] = true;
            }

            switch (this.ZoneType)
            {
                case ZoneType.Primary:
                    if (this.ResponsiblePerson == String.Empty)
                    {
                        this.ResponsiblePerson = String.Format("hostmaster.{0}", this.ZoneName);
                    }
                    inParams["AdminEmailName"] = this.ResponsiblePerson;
                    break;
                case ZoneType.Secondary:
                case ZoneType.Stub:
                case ZoneType.Forwarder:
                    if (this.MasterServer[0] == String.Empty)
                    {
                        ThrowTerminatingError(
                            new ErrorRecord(
                                new ArgumentException("Must specify Master IP Address(es)"),
                                "InvalidArgument",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    inParams["IpAddr"] = MasterServer;
                    break;
            }

            if (ShouldProcess(String.Format("Creating {0} zone for {1}.", this.ZoneType, this.ZoneName)))
            {
                ManagementBaseObject outParams = WmiZoneClass.InvokeMethod(
                    "CreateZone", inParams, null);

                NewPath = (String)outParams["RR"];
                if (PassThru)
                {
                    this.SetManagementPath(NewPath);
                    ManagementObject WmiZone = this.Get();

                    WriteObject(new Zone(WmiZone, Server));
                }
            }
        }
    }
}
