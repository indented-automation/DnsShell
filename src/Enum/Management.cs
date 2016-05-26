using System;

namespace DnsShell
{
    // Defines constants used by DnsShell for server management

    //
    // Enumerations for the Registry Provider
    //

    internal enum RegistryHive : uint
    {
        HKCR = 2147483628,    // HKEY_CLASSES_ROOT
        HKCU = 2147483649,    // HKEY_CURRENT_USER
        HKLM = 2147483650,    // HKEY_LOCAL_MACHINE
        HKU  = 2147483651,    // HKEY_USERS
        HKCC = 2147483653     // HKEY_CURRENT_CONFIG
    }

    //
    // WMI Resource Record Types
    //

    //public enum WmiRecordType : ushort
    //{
    //    A          = 1,     // a host address                              [RFC1035]
    //    NS         = 2,     // an authoritative name server                [RFC1035]
    //    MD         = 3,     // a mail destination (Obsolete - use MX)      [RFC1035]
    //    MF         = 4,     // a mail forwarder (Obsolete - use MX)        [RFC1035]
    //    CNAME      = 5,     // the canonical name for an alias             [RFC1035]
    //    SOA        = 6,     // marks the start of a zone of authority      [RFC1035]
    //    MB         = 7,     // a mailbox domain name (EXPERIMENTAL)        [RFC1035]
    //    MG         = 8,     // a mail group member (EXPERIMENTAL)          [RFC1035]
    //    MR         = 9,     // a mail rename domain name (EXPERIMENTAL)    [RFC1035]
    //    WKS        = 11,    // a well known service description            [RFC1035]
    //    PTR        = 12,    // a domain name pointer                       [RFC1035]
    //    HINFO      = 13,    // host information                            [RFC1035]
    //    MINFO      = 14,    // mailbox or mail list information            [RFC1035]
    //    MX         = 15,    // mail exchange                               [RFC1035]
    //    TXT        = 16,    // text strings                                [RFC1035]
    //    RP         = 17,    // for Responsible Person                      [RFC1183]
    //    AFSDB      = 18,    // for AFS Data Base location                  [RFC1183]
    //    X25        = 19,    // for X.25 PSDN address                       [RFC1183]
    //    ISDN       = 20,    // for ISDN address                            [RFC1183]
    //    RT         = 21,    // for Route Through                           [RFC1183]
    //    SIG        = 24,    // for security signature                      [RFC4034][RFC3755][RFC2535]
    //    KEY        = 25,    // for security key                            [RFC4034][RFC3755][RFC2535]
    //    AAAA       = 28,    // IP6 Address                                 [RFC3596]
    //    NXT        = 30,    // Next Domain - OBSOLETE                      [RFC3755][RFC2535]
    //    SRV        = 33,    // Server Selection                            [RFC2782]
    //    ATMA       = 34,    // ATM Address                                 [ATMDOC]
    //    WINS       = 65281, // WINS records (WINS Lookup record)           [MS DNS]
    //    WINSR      = 65282  // WINSR records (WINS Reverse Lookup record)  [MS DNS]
    //}

    internal enum WmiRecordClass : ushort
    {
        MicrosoftDNS_AAAAType  = 28,    // Represents an IPv6 Address (AAAA), often pronounced quad-A, RR
        MicrosoftDNS_AFSDBType = 18,    // Represents an Andrew File System Database Server (AFSDB) RR
        MicrosoftDNS_ATMAType  = 34,    // Represents an ATM Address-to-Name (ATMA) RR.
        MicrosoftDNS_AType     = 1,     // Represents an Address (A) RR
        MicrosoftDNS_CNAMEType = 5,     // Represents a Canonical Name (CNAME) RR
        MicrosoftDNS_HINFOType = 13,    // Represents a Host Information (HINFO) RR
        MicrosoftDNS_ISDNType  = 20,    // Represents an ISDN RR
        MicrosoftDNS_KEYType   = 25,    // Represents a KEY RR
        MicrosoftDNS_MBType	   = 7,     // Represents a Mailbox (MB) RR
        MicrosoftDNS_MDType	   = 3,     // Represents a Mail Agent for Domain (MD) RR
        MicrosoftDNS_MFType	   = 4,     // Represents a Mail Forwarding Agent for Domain (MF) RR
        MicrosoftDNS_MGType    = 8,     // Represents an MG RR
        MicrosoftDNS_MINFOType = 14,    // Represents an Mail Information (MINFO) RR
        MicrosoftDNS_MRType    = 9,     // Represents a Mailbox Rename (MR) RR
        MicrosoftDNS_MXType    = 15,    // Represents a Mail Exchanger (MX) RR
        MicrosoftDNS_NSType	   = 2,     // Represents a Name Server (NS) RR
        MicrosoftDNS_NXTType   = 30,    // Represents a Next (NXT) RR
        MicrosoftDNS_PTRType   = 12,    // Represents a Pointer (PTR) RR
        MicrosoftDNS_RPType    = 17,    // Represents a Responsible Person (RP) RR
        MicrosoftDNS_RTType    = 21,    // Represents a Route Through (RT) RR
        MicrosoftDNS_SIGType   = 24,    // Represents a Signature (SIG) RR
        MicrosoftDNS_SOAType   = 6,     // Represents a Start Of Authority (SOA) RR
        MicrosoftDNS_SRVType   = 33,    // Represents a Service (SRV) RR
        MicrosoftDNS_TXTType   = 16,    // Represents a Text (TXT) RR
        MicrosoftDNS_WINSType  = 65281, // Represents a WINS RR
        MicrosoftDNS_WINSRType = 65282, // Represents a WINS-Reverse (WINSR) RR
        MicrosoftDNS_WKSType   = 11,    // Represents a Well-Known Service (WKS) RR
        MicrosoftDNS_X25Type   = 19     // Represents an X.25 (X25) RR
    }

    //
    // Enumerations used by MicrosoftDNS_Zone
    //

    public enum ZoneType : uint
    {
        Hint      = 0,
        Primary   = 1,
        Secondary = 2,
        Stub      = 3,
        Forwarder = 4
    }

    public enum ZoneDynamicUpdate : uint
    {
        None       = 0,
        All        = 1,
        SecureOnly = 2
    }

    public enum ZoneTransfer : uint
    {
        Any  = 0,
        NS   = 1,
        List = 2,
        None = 3
    }

    public enum Notify : uint
    {
        None = 0,
        NS   = 1,
        List = 2
    }

    //
    // Enumerations used by MicrosoftDNS_Server
    //

    public enum ServerDynamicUpdate : uint
    {
        NoRestriction  = 0, // No Restrictions
        NoSOAUpdate    = 1, // Does not allow dynamic updates of SOA records
        NoRootNSUpdate = 2, // Does not allow dynamic updates of NS records at the zone root
        NoNSUpdate     = 4  // Does not allow dynamic updates of NS records not at the zone root (delegation NS records)
    }

    public enum AutoConfigZones : uint
    {
        None                     = 0,   // None
        AllowDynamicUpdateOnly   = 1,   // Only servers that allow dynamic updates
        AllowNoDynamicUpdateOnly = 2,   // Only servers that do not allow dynamic updates
        All                      = 4    // All Servers
    }

    public enum BootMethod : uint
    {
        Unitialised              = 0,   // Uninitialized
        FromFile                 = 1,   // Boot from file
        FromRegistry             = 2,   // Boot from registry
        FromDirectoryAndRegistry = 3    // Boot from directory and registry
    }

    public enum DnsSecMode : uint
    {
        None = 0,   // No DNSSEC records are included in the response unless the query requested a resource record set of the DNSSEC record type.
        All  = 1,   // DNSSEC records are included in the response according to RFC 2535.
        Opt  = 2    // DNSSEC records are included in a response only if the original client query contained the OPT resource record according to RFC 2671
    }

    public enum EventLogLevel : uint
    {
        None              = 0,  // None
        Errors            = 1,  // Log only errors
        ErrorsAndWarnings = 2,  // Log only warnings and errors.
        All               = 4   // Log all events.
    }

    public enum LogLevel : uint
    {
        None         = 0,
        Query        = 1,
        Notify       = 16,
        Update       = 32,
        NonQuery     = 254,
        Questions    = 256,
        Answers      = 512,
        Send         = 4096,
        Receive      = 8192,
        Udp          = 16384,
        Tcp          = 32768,
        AllPackets   = 65535,
        DSWrite      = 65536,
        DSUpdate     = 131072,
        FullPackets  = 16777216,
        WriteThrough = 2147483648
    }

    public enum NameCheckFlag : uint
    {
        StrictRFCANSI = 0,
        NonRFCANSI    = 1,
        MultibyteUTF8 = 2,
        AllNames = 3
    }

    [FlagsAttribute]
    public enum RpcProtocol : int
    {
        None       = 0,
        Tcp        = 1,
        NamedPipes = 2,
        Lpc        = 4
    }
}