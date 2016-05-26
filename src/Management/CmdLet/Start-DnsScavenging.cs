using System;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsLifecycle.Start, "DnsScavenging")]
    public class StartDnsScavenging : ManagementCmdlet
    {
        #region Parameter:Server (String[])
        [Parameter(
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Server",
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String[] Server = new String[] { "localhost" };
        #endregion

        #region CmdLet overrides
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
                        Server DnsServer = new Server(wmiServer);
                        DnsServer.StartScavenging();
                    }
                }
            }
        }
        #endregion
    }
}
