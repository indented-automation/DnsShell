using System;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Management.Automation;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "ADDnsRecord")]
    public class GetADDnsRecord : ADCmdLet
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

            String LdapFilter = "(&(objectCategory=dnsNode))";
            if (Name != String.Empty)
            {
                LdapFilter = String.Format("(&(objectCategory=dnsNode)(name={0}))", Name);
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
                    for (int i = 0; i < Entry.Attributes["dnsrecord"].Count; i++)
                    {
                        RecordType CurrentRecordType = (RecordType)(UInt16)(
                            ((Byte[])Entry.Attributes["dnsrecord"].GetValues(typeof(Byte[]))[i])[2] +
                            ((Byte[])Entry.Attributes["dnsrecord"].GetValues(typeof(Byte[]))[i])[3] * 256);

                        base.WriteRecord(Entry, i, CurrentRecordType);
                    }
                }

                if (PageResponse.Cookie.Length == 0)
                    break;

                PageRequest.Cookie = PageResponse.Cookie;
            }
        }
    }
}
