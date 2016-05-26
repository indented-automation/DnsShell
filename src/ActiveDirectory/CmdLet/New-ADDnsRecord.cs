using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Net;
using DnsShell.ActiveDirectory;

namespace DnsShell
{
    [Cmdlet(VerbsCommon.New, "ADDnsRecord")]
    public class NewADDnsRecord : Cmdlet
    {
        //#region Parameter:ZoneName (String) :: Parameter Set <All>
        //[Parameter(
        //    Mandatory = true,
        //    HelpMessage = "ZoneName",
        //    ParameterSetName = "")]
        //[Alias("ContainerName")]
        //public String ZoneName;
        //#endregion

        //#region Parameter:RecordType (RecordType) :: Parameter Set <All>
        //[Parameter(
        //    Mandatory = true,
        //    ValueFromPipelineByPropertyName = true,
        //    HelpMessage = "RecordType",
        //    ParameterSetName = "")]
        //[Alias("Type")]
        //public RecordType RecordType;
        //#endregion

        //#region Parameter:Name (String) :: Parameter Set All but 15 and 16
        //[Alias("NodeName")]
        //[Parameter(ParameterSetName = "PS1")]
        //[Parameter(ParameterSetName = "PS2")]
        //[Parameter(ParameterSetName = "PS3")]
        //[Parameter(ParameterSetName = "PS4")]
        //[Parameter(ParameterSetName = "PS5")]
        //[Parameter(ParameterSetName = "PS6")]
        //[Parameter(ParameterSetName = "PS7")]
        //[Parameter(ParameterSetName = "PS8")]
        //[Parameter(ParameterSetName = "PS9")]
        //[Parameter(ParameterSetName = "PS10")]
        //[Parameter(ParameterSetName = "PS11")]
        //[Parameter(ParameterSetName = "PS12")]
        //[Parameter(ParameterSetName = "PS14")]
        //public String Name = String.Empty;
        //#endregion

        //#region Parameter:Server (String) :: Parameter Set <All>
        //[Parameter(
        //    ValueFromPipelineByPropertyName = true,
        //    ParameterSetName = "")]
        //[Alias("ComputerName", "ServerName")]
        //public String Server = "localhost";
        //#endregion

        //#region Parameter:TTL (UInt32) :: Parameter Set <All>
        //[Parameter(ParameterSetName = "")]
        //[ValidateRange(0, 2147483647)] // RFC 2181
        //public UInt32 TTL = 0;
        //#endregion

        //#region Parameter:IPAddress (IPAddress) :: Parameter Set 1 (A, AAAA, WKS)
        //[Parameter(ParameterSetName = "PS1")]
        //[Alias("Address")]
        //public IPAddress IPAddress;
        //#endregion

        //#region Parameter:IPProtocol (String) :: Parameter Set 1 (WKS)
        //[Parameter(ParameterSetName = "PS1")]
        //[ValidateSet("tcp", "udp")]
        //public String IPProtocol = "tcp";
        //#endregion

        //#region Parameter:Services (String[]) :: Parameter Set 1 (WKS)
        //[Parameter(ParameterSetName = "PS1")]
        //public String[] Services = new String[] { String.Empty };
        //#endregion

        //#region Parameter:Hostname (String) :: Parameter Set 2 (CNAME, MB, MD, MF, NS, PTR)
        //[Parameter(ParameterSetName = "PS2")]
        //public string Hostname = String.Empty;
        //#endregion

        //#region Parameter:MailboxName (String) :: Parameter Set 3 (MG, MR)
        //[Parameter(ParameterSetName = "PS3")]
        //public string MailboxName = String.Empty;
        //#endregion

        //#region Parameter:Text (String) :: Parameter Set 4 (TXT)
        //[Parameter(ParameterSetName = "PS4")]
        //public string Text = String.Empty;
        //#endregion

        //#region Parameter:CPU (String) :: Parameter Set 5 (HINFO)
        //[Parameter(ParameterSetName = "PS5")]
        //public string CPU = String.Empty;
        //#endregion

        //#region Parameter:OS (String) :: Parameter Set 5 (HINFO)
        //[Parameter(ParameterSetName = "PS5")]
        //public string OS = String.Empty;
        //#endregion

        //#region Parameter:ISDNNumber (String) :: Parameter Set 6 (ISDN)
        //[Parameter(ParameterSetName = "PS6")]
        //public string ISDNNumber = String.Empty;
        //#endregion

        //#region Parameter:SubAddress (String) :: Parameter Set 6 (ISDN)
        //[Parameter(ParameterSetName = "PS6")]
        //public string SubAddress = String.Empty;
        //#endregion

        //#region Parameter:ResponsibleMailbox (String) :: Parameter Set 7 and 9 (MINFO, RP)
        //[Parameter(ParameterSetName = "PS7")]
        //[Parameter(ParameterSetName = "PS9")]
        //public string ResponsibleMailbox = String.Empty;
        //#endregion

        //#region Parameter:ErrorMailbox (String) :: Parameter Set 7 (MINFO)
        //[Parameter(ParameterSetName = "PS7")]
        //public String ErrorMailbox = String.Empty;
        //#endregion

        //#region Parameter:TXTDomainName (String) :: Parameter Set 9 (RP)
        //[Parameter(ParameterSetName = "PS9")]
        //public string TXTDomainName = String.Empty;
        //#endregion

        //#region Parameter:TargetName (String) :: Parameter Set 10, 11 and 14 (AFSDB, MX, RT, SRV)
        //[Parameter(ParameterSetName = "PS10")]
        //[Parameter(ParameterSetName = "PS11")]
        //[Parameter(ParameterSetName = "PS14")]
        //public string TargetName = String.Empty;
        //#endregion

        //#region Parameter:SubType (UInt16) :: Parameter Set 10 (AFSDB)
        //[Parameter(ParameterSetName = "PS10")]
        //public UInt16 SubType = UInt16.MaxValue;
        //#endregion

        //#region Parameter:Preference (UInt16) :: Parameter Set 11 (MX, RT)
        //[Parameter(ParameterSetName = "PS11")]
        //public UInt16 Preference = UInt16.MaxValue;
        //#endregion

        //#region Parameter:PSDNAddress (String) :: Parameter Set 12 (X25)
        //[Parameter(ParameterSetName = "PS12")]
        //public String PSDNAddress = String.Empty;
        //#endregion

        //#region Parameter:Priority (UInt16) :: Parameter Set 14 (SRV)
        //[Parameter(ParameterSetName = "PS14")]
        //public UInt16 Priority = 0;
        //#endregion

        //#region Parameter:Weight (UInt16) :: Parameter Set 14 (SRV)
        //[Parameter(ParameterSetName = "PS14")]
        //public UInt16 Weight = 100;
        //#endregion

        //#region Parameter:Port (UInt16) :: Parameter Set 14 (SRV)
        //[Parameter(ParameterSetName = "PS14")]
        //public UInt16 Port = UInt16.MaxValue;
        //#endregion

        //#region Parameter:MappingFlag (WINSMappingFlag) :: Parameter Set 15 and 16 (WINS, WINSR)
        //[Parameter(ParameterSetName = "PS15")]
        //[Parameter(ParameterSetName = "PS16")]
        //public WINSMappingFlag MappingFlag = WINSMappingFlag.Replication;
        //#endregion

        //#region Parameter:LookupTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        //[Parameter(ParameterSetName = "PS15")]
        //[Parameter(ParameterSetName = "PS16")]
        //public UInt32 LookupTimeout = 2;
        //#endregion

        //#region Parameter:CacheTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        //[Parameter(ParameterSetName = "PS15")]
        //[Parameter(ParameterSetName = "PS16")]
        //public UInt32 CacheTimeout = 900;
        //#endregion

        //#region Parameter:WinsServers (String[]) :: Parameter Set 15 (WINS)
        //[Parameter(ParameterSetName = "PS15")]
        //public String[] WinsServers = new String[] { String.Empty };
        //#endregion

        //#region Parameter:ResultDomain (String) :: Parameter Set 16 (WINSR)
        //[Parameter(ParameterSetName = "PS16")]
        //public String ResultDomain;
        //#endregion

        //#region Parameter:PassThru (Switch) :: Parameter Set <All>
        //[Parameter(ParameterSetName = "")]
        //public SwitchParameter PassThru;
        //#endregion

        protected override void ProcessRecord()
        {
            // Validate parent container

            // Get the Serial from the SOA for the parent container

            //String DN = "OU=test,DC=domain,DC=com";
            //DirectoryEntry ParentContainer = new DirectoryEntry("LDAP://" + DN);

            // Create the dnsRecord array

            A NewRecord = new A();
            NewRecord.Version = 5;
            NewRecord.TTL = 3600;
            NewRecord.UpdatedAtSerial = 35832;
            NewRecord.RecordType = RecordType.A;
            NewRecord.IPAddress = IPAddress.Parse("1.2.3.4");
            NewRecord.TimeStamp = DateTime.Now;
            NewRecord.Rank = Rank.Zone;
            NewRecord.RDataLength = 4;
            NewRecord.Flags = 0;

            WriteObject(NewRecord);

            // Validate dnsNode object
            
            // Create the object

            // Load the record
        }
    }
}
