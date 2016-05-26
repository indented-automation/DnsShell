using System;
using System.Management;
using System.Management.Automation;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    /// <summary>
    /// Writes changes to the Root Hints file back to the DNS Cache File.
    /// </summary>
    [Cmdlet(VerbsCommunications.Write, "RootHints")]
    public class WriteRootHints : ManagementCmdlet
    {
        #region Parameter:Server (String[])
        /// <summary>
        /// Specified the  the ServerName(s) to operate on. Defaults to localhost.
        /// </summary>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Server",
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public string[] Server = new string[] { "localhost" };
        #endregion

        #region Parameter:PassThru (Switch)
        /// <summary>
        /// Specify the PassThru parameter that allows the user to specify 
        /// that the cmdlet should pass the cache object down the pipeline.
        /// </summary>
        [Parameter(HelpMessage = "PassThru")]
        public SwitchParameter PassThru;
        #endregion

        #region CmdLet overrides
        /// <summary>
        /// Process record override.
        /// </summary>
        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetManagementPath("MicrosoftDNS_RootHints");

            foreach (String ServerName in Server)
            {
                this.SetScope(ServerName);

                ManagementObjectCollection wmiResults = this.Search("");
                if (wmiResults != null)
                {
                    foreach (ManagementObject wmiRootHints in wmiResults)
                    {
                        RootHints DnsRootHints = new RootHints(wmiRootHints);
                        DnsRootHints.WriteBackRootHintDatafile();

                        if (this.PassThru)
                        {
                            WriteObject(DnsRootHints);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
