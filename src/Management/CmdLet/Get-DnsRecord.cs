using System;
using System.Collections;
using System.Management;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "DnsRecord",
        DefaultParameterSetName = "PS1")]
    public class DnsRecord : ManagementCmdlet
    {
        #region Parameter:Name (RegEx String) :: Parameter Set <All>
        [Parameter(Position = 0, ParameterSetName = "")]
        [Alias("RecordName", "OwnerName")]
        public String Name = ".*";
        #endregion

        #region Parameter:RecordType (RecordType[]) :: Parameter Set <All>
        /// <summary>
        /// .PARAMETER:Record type, values defined by the DnsShell.WmiRecordType enumeration.
        /// </summary>
        [Parameter(Position = 1, ParameterSetName = "")]
        [Alias("Type")]
        public RecordType[] RecordType;
        #endregion

        #region Parameter:ZoneName (String) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1", ValueFromPipelineByPropertyName = true)]
        [Alias("ContainerName")]
        public String ZoneName = String.Empty;
        #endregion

        #region Parameter:Cache (Switch) :: Parameter Set 2
        /// <summary>
        /// .PARAMETER:Only return records from the DNS Cache.
        /// </summary>
        [Parameter(ParameterSetName = "PS2")]
        public SwitchParameter Cache;
        #endregion

        #region Parameter:RootHints (Switch) :: Parameter Set 3
        /// <summary>
        /// .PARAMETER:Only return Root Hints
        /// </summary>
        [Parameter(ParameterSetName = "PS3")]
        public SwitchParameter RootHints;
        #endregion

        #region Parameter:Filter (String) :: Parameter Set 4
        /// <summary>
        /// .PARAMETER:A WQL based filter string. By default the filter exclues Root Hints and Cached records. If * is used a wildcard it will be replaced with %.
        /// </summary>
        [Parameter(ParameterSetName = "PS4")]
        public String Filter = "NOT ContainerName LIKE '..%'";
        #endregion

        #region Parameter:Server (String[]) :: Parameter Set <All>
        /// <summary>
        /// .PARAMETER:Specify the ServerName(s) to operate on. Default is localhost.
        /// </summary>
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public string[] Server = new string[] { "localhost" };
        #endregion

        #region CmdLet overrides
        protected override void ProcessRecord()
        {
            this.SetOptions();

            // Validate the Type parameter

            ArrayList WmiRecordClassNames = new ArrayList();
            if (RecordType == null)
            {
                foreach (String WmiRecordClass in Enum.GetNames(typeof(WmiRecordClass)))
                {
                    WmiRecordClassNames.Add(WmiRecordClass);
                }
            }
            else
            {
                foreach (RecordType RType in RecordType)
                {
                    WmiRecordClassNames.Add(Enum.GetName(typeof(WmiRecordClass), RType));
                }
            }

            // Build the Filter

            if (Cache)
            {
                Filter = "ContainerName='..Cache'";
            }
            else if (RootHints)
            {
                Filter = "ContainerName='..RootHint'";
            }
            else if (ZoneName != String.Empty)
            {
                Filter = String.Format("ContainerName='{0}'", ZoneName);
            }

            Regex NameRegEx = new Regex(Name, RegexOptions.IgnoreCase);

            foreach (String ServerName in Server)
            {
                this.ErrorLevel = 0;
                this.SetScope(ServerName);

                foreach (String WmiRecordClassName in WmiRecordClassNames)
                {
                    if (this.ErrorLevel == 0)
                    {
                        this.SetManagementPath(WmiRecordClassName);
                        ManagementObjectCollection wmiResults = this.Search(Filter);
                        if (wmiResults != null)
                        {
                            foreach (ManagementObject wmiRecord in wmiResults)
                            {
                                if (NameRegEx.IsMatch((String)wmiRecord.Properties["OwnerName"].Value))
                                {
                                    WriteRecord(wmiRecord);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
