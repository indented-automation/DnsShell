using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Management.Automation;
using DnsShell.ActiveDirectory;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(VerbsCommon.Get, "ADDnsPartition")]
    public class GetADDnsPartition : ADCmdLet
    {
        protected override void ProcessRecord()
        {
            DirectoryEntry RootDSE = new DirectoryEntry(
                String.Format("LDAP://{0}/RootDSE", Server));

            if (Credential != null)
            {
                RootDSE.Username = Credential.UserName;
                RootDSE.Password = System.Runtime.InteropServices.Marshal.PtrToStringAuto(
                    System.Runtime.InteropServices.Marshal.SecureStringToBSTR(Credential.Password));
            }

            String LdapFilter = "(&(objectCategory=crossRef)(!name=Enterprise Configuration)(!name=Enterprise Schema))";

            base.SetLdapConnection(Server, Credential);
            base.Properties = new String[] { "name",
                                             "whenCreated", 
                                             "whenChanged", 
                                             "objectGUID",
                                             "msDS-NC-Replica-Locations",
                                             "nCName",
                                             "nETBIOSName" };
            base.SetSearchRequest(RootDSE.Properties["configurationNamingContext"][0].ToString(), LdapFilter);


            int pageCount = 0;
            while (true)
            {
                // increment the pageCount by 1
                pageCount++;

                // cast the directory response into a SearchResponse object
                SearchResponse SearchResponse =
                   (SearchResponse)base.LdapConnection.SendRequest(this.SearchRequest);

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
                    WriteObject(new Partition(Entry));
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
    }
}
