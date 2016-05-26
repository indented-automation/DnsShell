using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Management.Automation;
using DnsShell.ActiveDirectory;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "ADDnsZone")]
    public class GetADDnsZone : ADCmdLet
    {
        #region Parameter:Name (String) :: Parameter Set <All>
        [Parameter(Position = 0)]
        [Alias("RecordName", "OwnerName")]
        public String Name = String.Empty;
        #endregion

        #region Parameter:SearchRoot (String) :: Parameter Set <All>
        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "")]
        [Alias("DN")]
        public String SearchRoot = String.Empty;
        #endregion

        protected override void ProcessRecord()
        {
            SearchRoot = base.CheckSearchRoot(SearchRoot);

            String LdapFilter = "(&(objectCategory=dnsZone))";
            if (Name != String.Empty)
            {
                LdapFilter = String.Format("(&(objectCategory=dnsZone)(name={0}))", Name);
            }

            base.SetLdapConnection(Server, Credential);
            base.SetSearchRequest(SearchRoot, LdapFilter);

            int pageCount = 0;
            while (true)
            {
                pageCount++;

                SearchResponse SearchResponse =
                   (SearchResponse)base.LdapConnection.SendRequest(this.SearchRequest);

                if (SearchResponse.Controls.Length != 1 ||
                    !(SearchResponse.Controls[0] is PageResultResponseControl))
                {
                    return;
                }

                PageResultResponseControl PageResponse =
                    (PageResultResponseControl)SearchResponse.Controls[0];

                foreach (SearchResultEntry Entry in SearchResponse.Entries)
                {
                    WriteObject(new Zone(Entry));
                }

                if (PageResponse.Cookie.Length == 0)
                    break;

                PageRequest.Cookie = PageResponse.Cookie;
            }
        }
    }
}
