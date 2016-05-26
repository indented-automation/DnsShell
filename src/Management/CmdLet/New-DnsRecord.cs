using System;
using System.Collections;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Text.RegularExpressions;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(
        VerbsCommon.New,
        "DnsRecord",
        SupportsShouldProcess = true,
        DefaultParameterSetName = "PS1")]
    public class NewDnsRecord : ManagementCmdlet
    {
        #region Parameter:ZoneName (String) :: Parameter Set <All>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("ContainerName")]
        public String ZoneName;
        #endregion

        #region Parameter:RecordType (RecordType) :: Parameter Set <All>
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "")]
        [Alias("Type")]
        public RecordType RecordType;
        #endregion

        #region Parameter:Name (String) :: Parameter Set All but 15 and 16
        [Alias("NodeName")]
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS1")]
        [Parameter(ParameterSetName = "PS2")]
        [Parameter(ParameterSetName = "PS3")]
        [Parameter(ParameterSetName = "PS4")]
        [Parameter(ParameterSetName = "PS5")]
        [Parameter(ParameterSetName = "PS6")]
        [Parameter(ParameterSetName = "PS7")]
        [Parameter(ParameterSetName = "PS8")]
        [Parameter(ParameterSetName = "PS9")]
        [Parameter(ParameterSetName = "PS10")]
        [Parameter(ParameterSetName = "PS11")]
        [Parameter(ParameterSetName = "PS12")]
        [Parameter(ParameterSetName = "PS14")]
        public String Name = String.Empty;
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        [Parameter(ParameterSetName = "", ValueFromPipelineByPropertyName = true)]
        [Alias("ComputerName", "ServerName")]
        public String Server = "localhost";
        #endregion

        #region Parameter:TTL (UInt32) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        [ValidateRange(0, 2147483647)] // RFC 2181
        public UInt32 TTL = 0;
        #endregion

        #region Parameter:IPAddress (IPAddress) :: Parameter Set 1 (A, AAAA, WKS)
        [Parameter(ParameterSetName = "PS1", ValueFromPipelineByPropertyName = true)]
        [Alias("Address")]
        public IPAddress IPAddress;
        #endregion

        #region Parameter:IPProtocol (String) :: Parameter Set 1 (WKS)
        [Parameter(ParameterSetName = "PS1", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("tcp", "udp")]
        public String IPProtocol = "tcp";
        #endregion

        #region Parameter:Services (String[]) :: Parameter Set 1 (WKS)
        [Parameter(ParameterSetName = "PS1", ValueFromPipelineByPropertyName = true)]
        public String[] Services = new String[] { String.Empty };
        #endregion

        #region Parameter:Hostname (String) :: Parameter Set 2 (CNAME, MB, MD, MF, NS, PTR)
        [Parameter(ParameterSetName = "PS2", ValueFromPipelineByPropertyName = true)]
        public string Hostname = String.Empty;
        #endregion

        #region Parameter:MailboxName (String) :: Parameter Set 3 (MG, MR)
        [Parameter(ParameterSetName = "PS3", ValueFromPipelineByPropertyName = true)]
        public string MailboxName = String.Empty;
        #endregion

        #region Parameter:Text (String) :: Parameter Set 4 (TXT)
        [Parameter(ParameterSetName = "PS4", ValueFromPipelineByPropertyName = true)]
        public string Text = String.Empty;
        #endregion

        #region Parameter:CPU (String) :: Parameter Set 5 (HINFO)
        [Parameter(ParameterSetName = "PS5", ValueFromPipelineByPropertyName = true)]
        public string CPU = String.Empty;
        #endregion

        #region Parameter:OS (String) :: Parameter Set 5 (HINFO)
        [Parameter(ParameterSetName = "PS5", ValueFromPipelineByPropertyName = true)]
        public string OS = String.Empty;
        #endregion

        #region Parameter:ISDNNumber (String) :: Parameter Set 6 (ISDN)
        [Parameter(ParameterSetName = "PS6", ValueFromPipelineByPropertyName = true)]
        public string ISDNNumber = String.Empty;
        #endregion

        #region Parameter:SubAddress (String) :: Parameter Set 6 (ISDN)
        [Parameter(ParameterSetName = "PS6", ValueFromPipelineByPropertyName = true)]
        public string SubAddress = String.Empty;
        #endregion

        #region Parameter:ResponsibleMailbox (String) :: Parameter Set 7 and 9 (MINFO, RP)
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS7")]
        [Parameter(ParameterSetName = "PS9")]
        public string ResponsibleMailbox = String.Empty;
        #endregion

        #region Parameter:ErrorMailbox (String) :: Parameter Set 7 (MINFO)
        [Parameter(ParameterSetName = "PS7", ValueFromPipelineByPropertyName = true)]
        public String ErrorMailbox = String.Empty;
        #endregion

        #region Parameter:TXTDomainName (String) :: Parameter Set 9 (RP)
        [Parameter(ParameterSetName = "PS9", ValueFromPipelineByPropertyName = true)]
        public string TXTDomainName = String.Empty;
        #endregion

        #region Parameter:TargetName (String) :: Parameter Set 10, 11 and 14 (AFSDB, MX, RT, SRV)
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS10")]
        [Parameter(ParameterSetName = "PS11")]
        [Parameter(ParameterSetName = "PS14")]
        public string TargetName = String.Empty;
        #endregion

        #region Parameter:SubType (UInt16) :: Parameter Set 10 (AFSDB)
        [Parameter(ParameterSetName = "PS10", ValueFromPipelineByPropertyName = true)]
        public UInt16 SubType = UInt16.MaxValue;
        #endregion

        #region Parameter:Preference (UInt16) :: Parameter Set 11 (MX, RT)
        [Parameter(ParameterSetName = "PS11", ValueFromPipelineByPropertyName = true)]
        public UInt16 Preference = UInt16.MaxValue;
        #endregion

        #region Parameter:PSDNAddress (String) :: Parameter Set 12 (X25)
        [Parameter(ParameterSetName = "PS12", ValueFromPipelineByPropertyName = true)]
        public String PSDNAddress = String.Empty;
        #endregion

        #region Parameter:Priority (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14", ValueFromPipelineByPropertyName = true)]
        public UInt16 Priority = 0;
        #endregion

        #region Parameter:Weight (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14", ValueFromPipelineByPropertyName = true)]
        public UInt16 Weight = 100;
        #endregion

        #region Parameter:Port (UInt16) :: Parameter Set 14 (SRV)
        [Parameter(ParameterSetName = "PS14", ValueFromPipelineByPropertyName = true)]
        public UInt16 Port = UInt16.MaxValue;
        #endregion

        #region Parameter:MappingFlag (WINSMappingFlag) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public WINSMappingFlag MappingFlag = WINSMappingFlag.Replication;
        #endregion

        #region Parameter:LookupTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public UInt32 LookupTimeout = 2;
        #endregion

        #region Parameter:CacheTimeout (UInt32) :: Parameter Set 15 and 16 (WINS, WINSR)
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Parameter(ParameterSetName = "PS15")]
        [Parameter(ParameterSetName = "PS16")]
        public UInt32 CacheTimeout = 900;
        #endregion

        #region Parameter:WinsServers (String[]) :: Parameter Set 15 (WINS)
        [Parameter(ParameterSetName = "PS15", ValueFromPipelineByPropertyName = true)]
        public String[] WinsServers = new String[] { String.Empty };
        #endregion

        #region Parameter:ResultDomain (String) :: Parameter Set 16 (WINSR)
        [Parameter(ParameterSetName = "PS16", ValueFromPipelineByPropertyName = true)]
        public String ResultDomain;
        #endregion

        #region Parameter:PassThru (Switch) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public SwitchParameter PassThru;
        #endregion

        protected override void ProcessRecord()
        {
            String WmiRecordClassName = Enum.GetName(typeof(WmiRecordClass), RecordType);

            #region Check / Fix Name parameter
            if (Name == "@" | Name == String.Empty) 
            { 
                Name = ZoneName; 
            }
            else if (RecordType == RecordType.PTR)
            {
                Regex IPMatch = new Regex(@"^\d{1,3}(?:\.\d{1,3}){3}$");
                if (IPMatch.IsMatch(Name))
                {
                    IPAddress IP = IPAddress.Parse(Name);
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Byte[] IPBytes = IP.GetAddressBytes();
                        Name = String.Format("{0}.{1}.{2}.{3}.in-addr.arpa.", 
                            IPBytes[3],
                            IPBytes[2],
                            IPBytes[1],
                            IPBytes[0]);
                    }
                }
            }
            if (!Name.Contains(ZoneName))
            {
                Name = Name.TrimEnd('.');
                Name = String.Format("{0}.{1}", Name, ZoneName);
            }
            #endregion

            #region Parameter Set validation
            Boolean Terminate = false;
            switch (ParameterSetName)
            {
                case "PS0": break;
                case "PS1":
                    if (WmiRecordClassName != "MicrosoftDNS_AType" &
                        WmiRecordClassName != "MicrosoftDNS_AAAAType" &
                        WmiRecordClassName != "MicrosoftDNS_WKSType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS2":
                    if (WmiRecordClassName != "MicrosoftDNS_CNAMEType" &
                        WmiRecordClassName != "MicrosoftDNS_MBType" &
                        WmiRecordClassName != "MicrosoftDNS_MDType" &
                        WmiRecordClassName != "MicrosoftDNS_MFType" &
                        WmiRecordClassName != "MicrosoftDNS_NSType" &
                        WmiRecordClassName != "MicrosoftDNS_PTRType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS3":
                    if (WmiRecordClassName != "MicrosoftDNS_MGType" &
                        WmiRecordClassName != "MicrosoftDNS_MRType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS4":
                    if (WmiRecordClassName != "MicrosoftDNS_TXTType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS5":
                    if (WmiRecordClassName != "MicrosoftDNS_HINFOType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS6":
                    if (WmiRecordClassName != "MicrosoftDNS_ISDNType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS7":
                    if (WmiRecordClassName != "MicrosoftDNS_MINFOType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS8":
                    if (WmiRecordClassName != "MicrosoftDNS_NXTType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS9":
                    if (WmiRecordClassName != "MicrosoftDNS_RPType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS10":
                    if (WmiRecordClassName != "MicrosoftDNS_AFSDBType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS11":
                    if (WmiRecordClassName != "MicrosoftDNS_MXType" &
                        WmiRecordClassName != "MicrosoftDNS_RTType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS12":
                    if (WmiRecordClassName != "MicrosoftDNS_X25Type")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS14":
                    if (WmiRecordClassName != "MicrosoftDNS_SRVType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS15":
                    if (WmiRecordClassName != "MicrosoftDNS_WINSType")
                    {
                        Terminate = true;
                    }
                    break;
                case "PS16":
                    if (WmiRecordClassName != "MicrosoftDNS_WINSRType")
                    {
                        Terminate = true;
                    }
                    break;
            }

            if (Terminate)
            {
                WriteError(
                    new ErrorRecord(
                        new PSInvalidOperationException("InvalidParameterSetForRecordType"),
                        "InvalidOperation",
                        ErrorCategory.InvalidOperation,
                        typeof(ManagementCmdlet)));
            }
            #endregion

            #region Extended parameters validation
            Terminate = false;
            switch (RecordType)
            {
                case RecordType.A:
                case RecordType.AAAA:
                    if (IPAddress == null)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("IP Address must be specified"),
                                "AddressRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.WKS:
                    if (IPAddress == null | IPProtocol == String.Empty | Services[0] == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("InternetAddress, IPProtocol and Services must be specified"),
                                "InternetAddressAndIPProtocolAndServicesRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.CNAME:
                case RecordType.MB:
                case RecordType.MD:
                case RecordType.MF:
                case RecordType.NS:
                case RecordType.PTR:
                    if (Hostname == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("Hostname must be specified"),
                                "HostnameRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.MG:
                case RecordType.MR:
                    if (MailboxName == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("MailboxName must be specified"),
                                "MailboxNameRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.TXT:
                    if (Text == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("Text must be specified"),
                                "TextRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.HINFO:
                    if (CPU == String.Empty & OS == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("CPU or OS must be specified"),
                                "CPUOrOSRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.ISDN:
                    if (ISDNNumber == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("ISDNNumber must be specified"),
                                "ISDNNumberRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.MINFO:
                    if (ResponsibleMailbox == String.Empty | ErrorMailbox == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("ResponsibleMailbox and ErrorMailbox must be specified"),
                                "ResponsibleMailboxAndErrorMailboxRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                //case RecordType.NXT:
                //    if (NextDomainName == String.Empty)
                //    {
                //        ThrowTerminatingError(
                //            new ErrorRecord(
                //                new PSInvalidOperationException("NextDomainName must be specified"),
                //                "NextDomainNameRequired",
                //                ErrorCategory.InvalidArgument,
                //                typeof(DnsShellManagementCmdlet)));
                //    }
                //    break;
                case RecordType.RP:
                    if (TXTDomainName == String.Empty | ResponsibleMailbox == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("TXTDomainName and ResponsibleMailbox must be specified"),
                                "TXTDomainNameAndResponsibleMailboxRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.AFSDB:
                    if (TargetName == String.Empty | SubType == UInt16.MaxValue)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("TargetName and SubType must be specified"),
                                "TargetNameRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.MX:
                case RecordType.RT:
                    if (TargetName == String.Empty | Preference == UInt16.MaxValue)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("TargetName and Preference must be specified"),
                                "TargetNameRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.SRV:
                    if (TargetName == String.Empty | Port == UInt16.MaxValue)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("TargetName and Port must be specified"),
                                "TargetNameAndResponsibleMailboxRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));                        
                    }
                    break;
                case RecordType.WINS:
                    if (WinsServers[0] == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("WinsServers must be specified"),
                                "WinsServersRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.WINSR:
                    if (ResultDomain == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("ResultDomain must be specified"),
                                "ResultDomainRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
                case RecordType.X25:
                    if (PSDNAddress == String.Empty)
                    {
                        WriteError(
                            new ErrorRecord(
                                new PSInvalidOperationException("PSDN Address must be specified"),
                                "PSDNAddressRequired",
                                ErrorCategory.InvalidArgument,
                                typeof(ManagementCmdlet)));
                    }
                    break;
            }
            #endregion

            if (ShouldProcess(String.Format("Name: {0} Type: {1} Zone: {2}", Name, RecordType, ZoneName)))
            {
                base.SetOptions();
                base.SetScope(Server);
                base.SetManagementPath("MicrosoftDNS_Server");

                String DnsServerName = String.Empty;

                ManagementObjectCollection WmiServers = base.Search("");
                foreach (ManagementObject WmiServer in WmiServers)
                {
                    DnsServerName = (String)WmiServer.Properties["Name"].Value;
                }

                this.SetManagementPath(WmiRecordClassName);

                ManagementObject WmiRecordClass = this.GetClass();
                String NewPath = String.Empty;

                ManagementBaseObject inParams = WmiRecordClass.GetMethodParameters("CreateInstanceFromPropertyData");
                inParams["DnsServerName"] = DnsServerName;
                inParams["ContainerName"] = ZoneName;
                inParams["OwnerName"] = Name;
                if (TTL != 0)
                {
                    inParams["TTL"] = TTL;
                }

                switch (WmiRecordClassName)
                {
                    case "MicrosoftDNS_AType":
                        if (IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            inParams["IPAddress"] = IPAddress;
                        }
                        else
                        {
                            WriteError(
                                new ErrorRecord(
                                    new PSInvalidOperationException("Invalid IP Address"),
                                    "InvalidIPAddress",
                                    ErrorCategory.InvalidArgument,
                                    typeof(ManagementCmdlet)));
                        }
                        break;
                    case "MicrosoftDNS_AAAAType":
                        if (IPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            inParams["IPv6Address"] = IPAddress;
                        }
                        else
                        {
                            WriteError(
                                new ErrorRecord(
                                    new PSInvalidOperationException("Invalid IPv6 Address"),
                                    "InvalidIPv6Address",
                                    ErrorCategory.InvalidArgument,
                                    typeof(ManagementCmdlet)));
                        }
                        break;
                    case "MicrosoftDNS_AFSDBType":
                        inParams["ServerName"] = TargetName;
                        inParams["Subtype"] = SubType;
                        break;
                    case "MicrosoftDNS_CNAMEType":
                        inParams["PrimaryName"] = Hostname;
                        break;
                    case "MicrosoftDNS_HINFOType":
                        inParams["CPU"] = CPU;
                        inParams["OS"] = OS;
                        break;
                    case "MicrosoftDNS_ISDNType":
                        inParams["ISDNNumber"] = ISDNNumber;
                        inParams["SubAddress"] = SubAddress;
                        break;
                    case "MicrosoftDNS_MBType":
                        inParams["MBHost"] = Hostname;
                        break;
                    case "MicrosoftDNS_MDType":
                        inParams["MDHost"] = Hostname;
                        break;
                    case "MicrosoftDNS_MFType":
                        inParams["MFHost"] = Hostname;
                        break;
                    case "MicrosoftDNS_MGType":
                        inParams["MGMailbox"] = MailboxName;
                        break;
                    case "MicrosoftDNS_MINFOType":
                        inParams["ResponibleMailbox"] = ResponsibleMailbox;
                        inParams["ErrorMailbox"] = ErrorMailbox;
                        break;
                    case "MicrosoftDNS_MRType":
                        inParams["MRMailbox"] = MailboxName;
                        break;
                    case "MicrosoftDNS_MXType":
                        inParams["MailExchange"] = TargetName;
                        inParams["Preference"] = Preference;
                        break;
                    case "MicrosoftDNS_NSType":
                        inParams["NSHost"] = Hostname;
                        break;
                    case "MicrosoftDNS_PTRType":
                        inParams["PTRDomainName"] = Hostname;
                        break;
                    case "MicrosoftDNS_RPType":
                        inParams["RPMailbox"] = ResponsibleMailbox;
                        inParams["TXTDomainName"] = TXTDomainName;
                        break;
                    case "MicrosoftDNS_RTType":
                        inParams["IntermediateHost"] = TargetName;
                        inParams["Preference"] = Preference;
                        break;
                    case "MicrosoftDNS_SRVType":
                        inParams["Port"] = Port;
                        inParams["Priority"] = Priority;
                        // This will fail against Windows 2003?
                        inParams["SRVDomainName"] = TargetName;
                        inParams["Weight"] = Weight;
                        break;
                    case "MicrosoftDNS_TXTType":
                        inParams["DescriptiveText"] = Text;
                        break;
                    case "MicrosoftDNS_WINSType":
                        foreach (String WinsServer in WinsServers)
                        {
                            if (!IsValidIPAddress(WinsServer))
                            {
                                WriteError(
                                    new ErrorRecord(
                                        new PSInvalidOperationException("Invalid IP Address"),
                                        "InvalidIPAddress",
                                        ErrorCategory.InvalidArgument,
                                        typeof(ManagementCmdlet)));
                            }
                        }
                        inParams["CacheTimeout"] = CacheTimeout;
                        inParams["LookupTimeout"] = LookupTimeout;
                        inParams["MappingFlag"] = MappingFlag;
                        inParams["WinsServers"] = String.Join(" ", WinsServers);
                        break;
                    case "MicrosoftDNS_WINSRType":
                        inParams["CacheTimeout"] = CacheTimeout;
                        inParams["LookupTimeout"] = LookupTimeout;
                        inParams["MappingFlag"] = MappingFlag;
                        inParams["ResultDomain"] = ResultDomain;
                        break;
                    case "MicrosoftDNS_WKSType":
                        inParams["InternetAddress"] = IPAddress;
                        inParams["IPProtocol"] = IPProtocol;
                        inParams["Services"] = String.Join(" ", Services);
                        break;
                    case "MicrosoftDNS_X25Type":
                        inParams["PSDNAddress"] = IPAddress;
                        break;
                    default:
                        WriteError(
                            new ErrorRecord(
                                new PSNotSupportedException("Unsupported Record Type"),
                                "NotSupported",
                                ErrorCategory.NotImplemented,
                                typeof(ManagementCmdlet)));
                    break;
                }

                ManagementBaseObject outParams = WmiRecordClass.InvokeMethod(
                    "CreateInstanceFromPropertyData", inParams, null);

                NewPath = (String)outParams["RR"];

                if (PassThru)
                {
                    this.SetManagementPath(NewPath);
                    ManagementObject WmiRecord = this.Get();

                    WriteRecord(WmiRecord);
                }
            }
        }
    }
}
