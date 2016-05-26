using System;
using System.Management.Automation;

namespace DnsShell
{
    [Cmdlet(VerbsCommon.Set, "ADDnsRecord")]
    public class SetADDnsRecord : Cmdlet
    {
        #region Parameter:Identity (String) :: Parameter Set <All>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Identity",
            ParameterSetName = "")]
        public String Identity;
        #endregion

        protected override void ProcessRecord()
        {

        }
    }
}
