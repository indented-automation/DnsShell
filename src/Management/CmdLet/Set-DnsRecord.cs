using System;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(
        VerbsCommon.Set,
        "DnsRecord",
        SupportsShouldProcess = true,
        DefaultParameterSetName = "PS0")]
    public class SetDnsRecord : ManagementCmdlet
    {
        #region Parameter:Identity (String) :: Parameter Set <All>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Identity", ParameterSetName = "")]
        [ValidatePattern(@"^\\\\.*\\root\\MicrosoftDNS:MicrosoftDNS_")]
        public String Identity;
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "Server", ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        #region Parameter:TTL (UInt32) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        [ValidateRange(0, 2147483647)] // RFC 2181
        public UInt32 TTL = 0;
        #endregion

        #region Parameter:Address (IPAddress) :: Parameter Set 1 (A, AAAA, WKS)
        [Parameter(ParameterSetName = "PS1")]
        [Alias("IPAddress")]
        public IPAddress Address;
        #endregion

        #region Parameter:IPProtocol (String) :: Parameter Set 1 (WKS)
        [Parameter(ParameterSetName = "PS1")]
        [ValidateSet("tcp", "udp")]
        public ProtocolType IPProtocol = ProtocolType.Tcp;
        #endregion

        #region Parameter:Services (String[]) :: Parameter Set 1 (WKS)
        [Parameter(ParameterSetName = "PS1")]
        public String[] Services = new String[] { String.Empty };
        #endregion

        #region Parameter:Hostname (String) :: Parameter Set 2 (CNAME, MB, MD, MF, NS, PTR)
        [Parameter(ParameterSetName = "PS2")]
        public string Hostname = String.Empty;
        #endregion

        #region Parameter:MailboxName (String) :: Parameter Set 3 (MG, MR)
        [Parameter(ParameterSetName = "PS3")]
        public string MailboxName = String.Empty;
        #endregion

        #region Parameter:Text (String) :: Parameter Set 4 (TXT)
        [Parameter(ParameterSetName = "PS4")]
        public string Text = String.Empty;
        #endregion

        #region Parameter:CPU (String) :: Parameter Set 5 (HINFO)
        [Parameter(ParameterSetName = "PS5")]
        public string CPU = String.Empty;
        #endregion

        #region Parameter:OS (String) :: Parameter Set 5 (HINFO)
        [Parameter(ParameterSetName = "PS5")]
        public string OS = String.Empty;
        #endregion

        #region Parameter:ISDNNumber (String) :: Parameter Set 6 (ISDN)
        [Parameter(ParameterSetName = "PS6")]
        public string ISDNNumber = String.Empty;
        #endregion

        #region Parameter:SubAddress (String) :: Parameter Set 6 (ISDN)
        [Parameter(ParameterSetName = "PS6")]
        public string SubAddress = String.Empty;
        #endregion

        #region Parameter:ResponsibleMailbox (String) :: Parameter Set 7, 9 and 13 (MINFO, RP)
        [Parameter(ParameterSetName = "PS7")]
        [Parameter(ParameterSetName = "PS9")]
        public string ResponsibleMailbox = String.Empty;
        #endregion

        #region Parameter:ErrorMailbox (String) :: Parameter Set 7 (MINFO)
        [Parameter(ParameterSetName = "PS7")]
        public String ErrorMailbox = String.Empty;
        #endregion

        #region Parameter:NextDomainName (String) :: Parameter Set 8 (NXT)
        [Parameter(ParameterSetName = "PS8")]
        public string NextDomainName = String.Empty;
        #endregion

        #region Parameter:Types (String) :: Parameter Set 8 (NXT)
        [Parameter(ParameterSetName = "PS8")]
        public string Types = String.Empty;
        #endregion

        #region Parameter:TXTDomainName (String) :: Parameter Set 9 (RP)
        [Parameter(ParameterSetName = "PS9")]
        public string TXTDomainName = String.Empty;
        #endregion

        #region Parameter:TargetName (String) :: Parameter Set 10, 11 and 14 (AFSDB, MX, RT, SRV)
        [Parameter(ParameterSetName = "PS10")]
        [Parameter(ParameterSetName = "PS11")]
        [Parameter(ParameterSetName = "PS14")]
        public string TargetName = String.Empty;
        #endregion

        #region Parameter:SubType (UInt16) :: Parameter Set 10 (AFSDB)
        [Parameter(ParameterSetName = "PS10")]
        public UInt16 SubType = UInt16.MaxValue;
        #endregion

        #region Parameter:Preference (UInt16) :: Parameter Set 11 (MX, RT)
        [Parameter(ParameterSetName = "PS11")]
        public UInt16 Preference = UInt16.MaxValue;
        #endregion

        #region Parameter:PSDNAddress (String) :: Parameter Set 12 (X25)
        [Parameter(ParameterSetName = "PS12")]
        public String PSDNAddress = String.Empty;
        #endregion

        #region Parameter:SOAServer (String) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public string SOAServer = String.Empty;
        #endregion
        
        #region Parameter:SerialNumber (UInt32) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public UInt32 SerialNumber = UInt32.MaxValue;
        #endregion

        #region Parameter:RefreshInterval (UInt32) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public UInt32 RefreshInterval = UInt32.MaxValue;
        #endregion

        #region Parameter:RetryDelay (UInt32) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public UInt32 RetryDelay = UInt32.MaxValue;
        #endregion

        #region Parameter:ExpireLimit (UInt32) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public UInt32 ExpireLimit = UInt32.MaxValue;
        #endregion

        #region Parameter:MinimumTTL (UInt32) :: Parameter Set 13 (SOA)
        [Parameter(ParameterSetName = "PS13")]
        public UInt32 MinimumTTL = UInt32.MaxValue;
        #endregion

        #region Parameter:Priority (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14")]
        public UInt16 Priority = 0;
        #endregion

        #region Parameter:Weight (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14")]
        public UInt16 Weight = 100;
        #endregion

        #region Parameter:Port (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14")]
        public UInt16 Port = UInt16.MaxValue;
        #endregion

        #region Parameter:MappingFlag (WINSMappingFlag) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public WINSMappingFlag MappingFlag = WINSMappingFlag.Replication;
        #endregion

        #region Parameter:LookupTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public UInt32 LookupTimeout = 2;
        #endregion

        #region Parameter:CacheTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public UInt32 CacheTimeout = 900;
        #endregion

        #region Parameter:WinsServers (String[]) :: Parameter Set 15 (WINS)
        [Parameter(ParameterSetName = "PS15")]
        public String[] WinsServers = new String[] { String.Empty };
        #endregion

        #region Parameter:ResultDomain (String) :: Parameter Set 16 (WINSR)
        [Parameter(ParameterSetName = "PS16")]
        public String ResultDomain;
        #endregion

        #region Parameter:PassThru (Switch) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public SwitchParameter PassThru;
        #endregion

        protected override void ProcessRecord()
        {
            this.SetOptions();
            this.SetScope(Server);
            this.SetManagementPath(Identity);
            ManagementObject wmiRecord = this.Get();

            WmiRecordClass WmiRecordClassName = (WmiRecordClass)Enum.Parse(typeof(WmiRecordClass), (String)wmiRecord.Properties["__CLASS"].Value);
            String NewPath = String.Empty;

            #region Parameter Set validation
            Boolean Terminate = false;
            switch (ParameterSetName)
            {
                case "PS0": break;
                case "PS1":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_AType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_AAAAType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_WKSType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS2":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_CNAMEType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MBType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MDType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MFType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_NSType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_PTRType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS3":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MGType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MRType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS4":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_TXTType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS5":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_HINFOType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS6":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_ISDNType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS7":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MINFOType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS8":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_NXTType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS9":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_RPType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS10":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_AFSDBType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS11":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_MXType &
                        WmiRecordClassName != WmiRecordClass.MicrosoftDNS_RTType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS12":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_X25Type)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS13":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_SOAType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS14":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_SRVType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS15":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_WINSType)
                    {
                        Terminate = true;
                    }
                    break;
                case "PS16":
                    if (WmiRecordClassName != WmiRecordClass.MicrosoftDNS_WINSRType)
                    {
                        Terminate = true;
                    }
                    break;
            }

            if (Terminate)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new PSInvalidOperationException("InvalidParameterSetForRecordType"),
                        "InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        typeof(ManagementCmdlet)));
            }
            #endregion

            if (ShouldProcess((String)wmiRecord.Properties["TextRepresentation"].Value))
            {
                switch (WmiRecordClassName)
                {
                    case WmiRecordClass.MicrosoftDNS_AType:
                        A ARecord = new A(wmiRecord, Server);
                        NewPath = ARecord.Modify(TTL, Address.ToString());
                        break;
                    case WmiRecordClass.MicrosoftDNS_AAAAType:
                        AAAA AAAARecord = new AAAA(wmiRecord, Server);
                        NewPath = AAAARecord.Modify(TTL, Address.ToString());
                        break;
                    case WmiRecordClass.MicrosoftDNS_AFSDBType:
                        AFSDB AFSDBRecord = new AFSDB(wmiRecord, Server);
                        NewPath = AFSDBRecord.Modify(TTL, SubType, TargetName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_CNAMEType:
                        CNAME CNAMERecord = new CNAME(wmiRecord, Server);
                        NewPath = CNAMERecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_HINFOType:
                        HINFO HINFORecord = new HINFO(wmiRecord, Server);
                        NewPath = HINFORecord.Modify(TTL, CPU, OS);
                        break;
                    case WmiRecordClass.MicrosoftDNS_ISDNType:
                        ISDN ISDNRecord = new ISDN(wmiRecord, Server);
                        NewPath = ISDNRecord.Modify(TTL, ISDNNumber, SubAddress);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MBType:
                        MB MBRecord = new MB(wmiRecord, Server);
                        NewPath = MBRecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MDType:
                        MD MDRecord = new MD(wmiRecord, Server);
                        NewPath = MDRecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MFType:
                        MF MFRecord = new MF(wmiRecord, Server);
                        NewPath = MFRecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MGType:
                        MG MGRecord = new MG(wmiRecord, Server);
                        NewPath = MGRecord.Modify(TTL, MailboxName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MINFOType:
                        MINFO MINFORecord = new MINFO(wmiRecord, Server);
                        NewPath = MINFORecord.Modify(TTL, ResponsibleMailbox, ErrorMailbox);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MRType:
                        MR MRRecord = new MR(wmiRecord, Server);
                        NewPath = MRRecord.Modify(TTL, MailboxName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_MXType:
                        MX MXRecord = new MX(wmiRecord, Server);
                        NewPath = MXRecord.Modify(TTL, Preference, TargetName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_NSType:
                        NS NSRecord = new NS(wmiRecord, Server);
                        NewPath = NSRecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_NXTType:
                        NXT NXTRecord = new NXT(wmiRecord, Server);
                        NewPath = NXTRecord.Modify(TTL, NextDomainName, Types);
                        break;
                    case WmiRecordClass.MicrosoftDNS_PTRType:
                        PTR PTRRecord = new PTR(wmiRecord, Server);
                        NewPath = PTRRecord.Modify(TTL, Hostname);
                        break;
                    case WmiRecordClass.MicrosoftDNS_RPType:
                        RP RPRecord = new RP(wmiRecord, Server);
                        NewPath = RPRecord.Modify(TTL, ResponsibleMailbox, TXTDomainName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_RTType:
                        RT RTRecord = new RT(wmiRecord, Server);
                        NewPath = RTRecord.Modify(TTL, Preference, TargetName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_SOAType:
                        SOA SOARecord = new SOA(wmiRecord, Server);
                        NewPath = SOARecord.Modify(TTL, SerialNumber,
                            SOAServer, ResponsibleMailbox, RefreshInterval,
                            RetryDelay, ExpireLimit, MinimumTTL);
                        break;
                    case WmiRecordClass.MicrosoftDNS_SRVType:
                        SRV SRVRecord = new SRV(wmiRecord, Server);
                        NewPath = SRVRecord.Modify(TTL, Priority, Weight, Port, TargetName);
                        break;
                    case WmiRecordClass.MicrosoftDNS_TXTType:
                        TXT TXTRecord = new TXT(wmiRecord, Server);
                        NewPath = TXTRecord.Modify(TTL, Text);
                        break;
                    case WmiRecordClass.MicrosoftDNS_WINSType:
                        WINS WINSRecord = new WINS(wmiRecord, Server);
                        NewPath = WINSRecord.Modify(TTL, MappingFlag, LookupTimeout,
                            CacheTimeout, String.Join(" ", WinsServers));
                        break;
                    case WmiRecordClass.MicrosoftDNS_WINSRType:
                        WINSR WINSRRecord = new WINSR(wmiRecord, Server);
                        NewPath = WINSRRecord.Modify(TTL, MappingFlag, LookupTimeout,
                            CacheTimeout, ResultDomain);
                        break;
                    case WmiRecordClass.MicrosoftDNS_WKSType:
                        WKS WKSRecord = new WKS(wmiRecord, Server);
                        NewPath = WKSRecord.Modify(Address, TTL, IPProtocol, String.Join(" ", Services));
                        break;
                    case WmiRecordClass.MicrosoftDNS_X25Type:
                        X25 X25Record = new X25(wmiRecord, Server);
                        NewPath = X25Record.Modify(TTL, PSDNAddress);
                        break;
                    default:
                        ThrowTerminatingError(
                            new ErrorRecord(
                                new PSNotSupportedException("Unsupported Record Type"),
                                "RecordModificationNotSupported",
                                ErrorCategory.NotImplemented,
                                typeof(ManagementCmdlet)));
                        break;
                }

                if (PassThru)
                {
                    this.SetManagementPath(NewPath);
                    wmiRecord = this.Get();

                    WriteRecord(wmiRecord);
                }
            }
        }
    }
}
