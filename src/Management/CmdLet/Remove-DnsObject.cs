using System;
using System.Management;
using System.Management.Automation;

namespace DnsShell.PowerShell.CmdLet
{
    /// <summary>
    /// .SYNOPSIS:Delete a DNS object.
    /// .DESCRIPTION:Deletes a the DNS object specified by the Identity (Management Path) and server.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "DnsObject", SupportsShouldProcess = true)]
    public class RemoveDnsObject : ManagementCmdlet
    {
        #region Parameter:Identity (String) :: Parameter Set <All>
        /// <summary>
        /// .PARAMETER:WMI Management Path used to identify the object. Accepts Identity Property returned by Get-DnsRecord or Get-DnsZone.
        /// </summary>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Identity",
            ParameterSetName = "")]
        [ValidatePattern(@"^\\\\.*\\root\\MicrosoftDNS:MicrosoftDNS_")]
        public String Identity;
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        /// <summary>
        /// .PARAMETER:Specify the ServerName to operate on. Defaults to localhost.
        /// </summary>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        #region Parameter:Force (Switch) :: Parameter Set <All>
        /// <summary>
        /// .PARAMETER:Bypass confirmation request.
        /// </summary>
        [Parameter(ParameterSetName = "")]
        public SwitchParameter Force;
        #endregion

        private Boolean YesToAll;
        private Boolean NoToAll;

        /// <summary>
        /// ProcessRecord Override
        /// </summary>
        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetScope(Server);
            this.SetManagementPath(Identity);

            ManagementObject wmiObject = this.Get();

            String ObjectName = String.Empty;
            if ((String)wmiObject.Properties["__CLASS"].Value == "MicrosoftDNS_Zone")
            {
                ObjectName = (String)wmiObject.Properties["Name"].Value;
            }
            else if (((String)wmiObject.Properties["__CLASS"].Value).Contains("Type"))
            {
                ObjectName = (String)wmiObject.Properties["OwnerName"].Value;
            }
            else
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new PSInvalidOperationException("Invalid object type"),
                        "InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        typeof(ManagementCmdlet)));
            }

            if (ShouldProcess(ObjectName))
            {
                if (Force || ShouldContinue(String.Format("Remove-DnsObject will delete {0}", ObjectName), "", ref YesToAll, ref NoToAll))
                {
                    wmiObject.Delete();
                }
            }
        }
    }
}
