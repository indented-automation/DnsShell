using System.Management.Automation;

namespace DnsShell.PowerShell.CmdLet
{
    /// <summary>
    /// .SYNOPSIS Delete a DNS Zone from a DNS Server using WMI.
    /// .DESCRIPTION Permanently delete a DNS Zone from the specified server.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "DnsZone")]
    public class RemoveDnsZone : ManagementCmdlet
    {
        /// <summary>
        /// ProcessRecord override
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }
    }
}
