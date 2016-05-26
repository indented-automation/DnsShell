using System;
using System.Collections;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "DnsZone", DefaultParameterSetName = "Forward")]
    public class GetDnsZone : ManagementCmdlet
    {
        #region Parameter:Name (String) :: Parameter Set <All>
        [Parameter(Position = 0, ParameterSetName = "")]
        [Alias("ZoneName", "ContainerName")]
        public String Name = String.Empty;
        #endregion

        #region Parameter:ZoneType (ZoneType) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        [Alias("Type")]
        public ZoneType ZoneType;
        #endregion
        
        #region Parameter:Filter (String) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public String Filter = String.Empty;
        #endregion

        #region Parameter:Server (String[]) :: Parameter Set <All>
        [Parameter(ParameterSetName = "", ValueFromPipelineByPropertyName = true)]
        [Alias("ComputerName", "ServerName")]
        public string[] Server = new string[] { "localhost" };
        #endregion

        #region Parameter:Forward (SwitchParameter) :: ParameterSet Forward
        [Parameter(ParameterSetName = "Forward")]
        public SwitchParameter Forward;
        #endregion

        #region Parameter:Forward (SwitchParameter) :: ParameterSet Forward
        [Parameter(ParameterSetName = "Reverse")]
        public SwitchParameter Reverse;
        #endregion

        #region CmdLet overrides
        protected override void ProcessRecord()
        {
            ArrayList FilterParts = new ArrayList();
            if (Name != String.Empty)
            {
                Name = Name.Replace('*', '%');
                FilterParts.Add(String.Format("ContainerName LIKE '{0}'", Name));
            }

            if (base.MyInvocation.BoundParameters.ContainsKey("ZoneType"))
            {
                FilterParts.Add(String.Format("ZoneType='{0}'", (uint)ZoneType));
            }
            if (base.MyInvocation.BoundParameters.ContainsKey("Filter"))
            {
                FilterParts.Add(Filter);
            }
            if (this.Forward)
            {
                FilterParts.Add("Reverse=false");
            }
            if (this.Reverse)
            {
                FilterParts.Add("Reverse=true");
            }
            
            Filter = String.Join(" AND ", (String[])FilterParts.ToArray(typeof(String)));

            this.SetOptions();
            this.SetManagementPath("MicrosoftDNS_Zone");

            foreach (String ServerName in Server)
            {
                base.SetScope(ServerName);

                ManagementObjectCollection wmiResults = this.Search(Filter);
                if (wmiResults != null)
                {
                    foreach (ManagementObject wmiZone in wmiResults)
                    {
                        WriteObject(new Zone(wmiZone, ServerName));
                    }
                }
            }
        }
        #endregion
    }
}
