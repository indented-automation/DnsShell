using System;
using System.DirectoryServices.Protocols;
using System.Management.Automation;
using System.Net;
using DnsShell.ActiveDirectory;

namespace DnsShell.PowerShell.CmdLet
{
    public class ADCmdLet : PSCmdlet
    {
        #region Parameter:Credential (PSCredential)
        [Parameter(ParameterSetName = "")]
        public PSCredential Credential;
        #endregion

        #region Parameter:Server (String)
        [Parameter(
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Server")]
        [Alias("ComputerName", "ServerName")]
        public String Server = Environment.GetEnvironmentVariable("UserDnsDomain");
        #endregion

        internal LdapConnection LdapConnection;
        internal SearchRequest SearchRequest;
        internal PageResultRequestControl PageRequest;
        internal String[] Properties = new String[] {
            "name",
            "whenCreated", 
            "whenChanged", 
            "dnsRecord", 
            "dnsProperty",
            "objectGUID" };

        internal String CheckSearchRoot(String SearchRoot)
        {
            if (SearchRoot == String.Empty)
            {
                String DnsDomain = Environment.GetEnvironmentVariable("UserDnsDomain");
                if (DnsDomain != null)
                {
                    SearchRoot = String.Format("DC=DomainDnsZones,DC={0}", DnsDomain.ToLower().Replace(".", ",DC="));
                }
                else
                {
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new PSArgumentException("Invalid Search Root"),
                            "InvalidSearchRoot",
                            ErrorCategory.InvalidArgument,
                            typeof(ADCmdLet)));
                }
            }
            return SearchRoot;
        }

        internal void SetLdapConnection(String ServerName, PSCredential Credential)
        {
            this.LdapConnection = new LdapConnection(ServerName);

            if (Credential != null)
            {
                String Password = System.Runtime.InteropServices.Marshal.PtrToStringAuto(
                            System.Runtime.InteropServices.Marshal.SecureStringToBSTR(Credential.Password));

                if (Credential.UserName.Contains("\\"))
                {
                    String[] Temp = Credential.UserName.Split('\\');
                    if (Temp[0] != String.Empty)
                    {
                        this.LdapConnection.Credential = new NetworkCredential(
                            Temp[1],
                            Password,
                            Temp[0]);
                    }
                    else
                    {
                        this.LdapConnection.Credential = new NetworkCredential(
                            Temp[1],
                            Password);
                    }
                }
                else
                {
                    this.LdapConnection.Credential = new NetworkCredential(
                        Credential.UserName, 
                        Password);
                }
            }
        }

        internal void SetSearchRequest(String SearchRoot, String LdapFilter)
        {
            this.SearchRequest = new SearchRequest();
            this.SearchRequest.DistinguishedName = SearchRoot;
            this.SearchRequest.Filter = LdapFilter;
            this.SearchRequest.Scope = SearchScope.Subtree;
            this.SearchRequest.Attributes.AddRange(this.Properties);

            this.PageRequest = new PageResultRequestControl(1000);
            this.SearchRequest.Controls.Add(this.PageRequest);
            SearchOptionsControl SearchOptions =
                new SearchOptionsControl(SearchOption.DomainScope);
            this.SearchRequest.Controls.Add(SearchOptions);
        }

        internal void FindAll()
        {
            // The code for this section is taken from http://msdn.microsoft.com/en-us/library/bb332056.aspx

            int pageCount = 0;
            while (true)
            {
                // increment the pageCount by 1
                pageCount++;

                // cast the directory response into a SearchResponse object
                SearchResponse SearchResponse =
                   (SearchResponse)this.LdapConnection.SendRequest(this.SearchRequest);

                // verify support for this advanced search operation
                 if (SearchResponse.Controls.Length != 1 ||
                     !(SearchResponse.Controls[0] is PageResultResponseControl))
                {
                    // Console.WriteLine("The server cannot page the result set");
                    return;
                }

                // cast the diretory control into a PageResultResponseControl object.
                PageResultResponseControl PageResponse =
                    (PageResultResponseControl)SearchResponse.Controls[0];

                // display the entries within this page
                foreach (SearchResultEntry Entry in SearchResponse.Entries)
                {
                }

                // if this is true, there are no more pages to request
                if (PageResponse.Cookie.Length == 0)
                    break;

                // set the cookie of the pageRequest equal to the cookie 
                // of the pageResponse to request the next page of data
                // in the send request
                PageRequest.Cookie = PageResponse.Cookie;
            }
        }

        internal void WriteRecord(SearchResultEntry Entry, Int32 Index, RecordType RecordType)
        {
            switch (RecordType)
            {
                case RecordType.A:
                    WriteObject(new A(Entry, Index));
                    break;
                case RecordType.AAAA:
                    WriteObject(new AAAA(Entry, Index));
                    break;
                case RecordType.AFSDB:
                    WriteObject(new AFSDB(Entry, Index));
                    break;
                case RecordType.ATMA:
                    WriteObject(new ATMA(Entry, Index));
                    break;
                case RecordType.CNAME:
                    WriteObject(new CNAME(Entry, Index));
                    break;
                case RecordType.HINFO:
                    WriteObject(new HINFO(Entry, Index));
                    break;
                case RecordType.MX:
                    WriteObject(new MX(Entry, Index));
                    break;
                case RecordType.NS:
                    WriteObject(new NS(Entry, Index));
                    break;
                case RecordType.SOA:
                    WriteObject(new SOA(Entry, Index));
                    break;
                case RecordType.SRV:
                    WriteObject(new SRV(Entry, Index));
                    break;
                default: break;
            }
        }
    }
}
