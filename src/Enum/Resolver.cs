using System;

namespace DnsShell
{
    // Defines constants used by DnsShell.Resolver
    //
    // The majority of parameters are documented by IANA: http://www.iana.org/assignments/dns-parameters
    // Additional parameters include the WINS RRType and WINS Mapping Flags (Replication Flags)

    public enum RecordClass : ushort
    {
        IN         = 1,     //                                             [RFC1035]
        CH         = 3,     //                                             [Moon1981]
        HS         = 4,     //                                             [Dyer1987]
        NONE       = 254,   //                                             [RFC2136] 
        ANY        = 255    //                                             [RFC1035]
    }

    public enum RecordType : ushort
    {
        A          = 1,     // a host address                              [RFC1035]
        NS         = 2,     // an authoritative name server                [RFC1035]
        MD         = 3,     // a mail destination (Obsolete - use MX)      [RFC1035]
        MF         = 4,     // a mail forwarder (Obsolete - use MX)        [RFC1035]
        CNAME      = 5,     // the canonical name for an alias             [RFC1035]
        SOA        = 6,     // marks the start of a zone of authority      [RFC1035]
        MB         = 7,     // a mailbox domain name (EXPERIMENTAL)        [RFC1035]
        MG         = 8,     // a mail group member (EXPERIMENTAL)          [RFC1035]
        MR         = 9,     // a mail rename domain name (EXPERIMENTAL)    [RFC1035]
        NULL       = 10,    // a null RR (EXPERIMENTAL)                    [RFC1035]
        WKS        = 11,    // a well known service description            [RFC1035]
        PTR        = 12,    // a domain name pointer                       [RFC1035]
        HINFO      = 13,    // host information                            [RFC1035]
        MINFO      = 14,    // mailbox or mail list information            [RFC1035]
        MX         = 15,    // mail exchange                               [RFC1035]
        TXT        = 16,    // text strings                                [RFC1035]
        RP         = 17,    // for Responsible Person                      [RFC1183]
        AFSDB      = 18,    // for AFS Data Base location                  [RFC1183]
        X25        = 19,    // for X.25 PSDN address                       [RFC1183]
        ISDN       = 20,    // for ISDN address                            [RFC1183]
        RT         = 21,    // for Route Through                           [RFC1183]
        NSAP       = 22,    // for NSAP address, NSAP style A record       [RFC1706]
        NSAPPTR    = 23,    // for domain name pointer, NSAP style         [RFC1348] 
        SIG        = 24,    // for security signature                      [RFC4034][RFC3755][RFC2535]
        KEY        = 25,    // for security key                            [RFC4034][RFC3755][RFC2535]
        PX         = 26,    // X.400 mail mapping information              [RFC2163]
        GPOS       = 27,    // Geographical Position                       [RFC1712]
        AAAA       = 28,    // IP6 Address                                 [RFC3596]
        LOC        = 29,    // Location Information                        [RFC1876]
        NXT        = 30,    // Next Domain - OBSOLETE                      [RFC3755][RFC2535]
        EID        = 31,    // Endpoint Identifier                         [Patton]
        NIMLOC     = 32,    // Nimrod Locator                              [Patton]
        SRV        = 33,    // Server Selection                            [RFC2782]
        ATMA       = 34,    // ATM Address                                 [ATMDOC]
        NAPTR      = 35,    // Naming Authority Pointer                    [RFC2915][RFC2168]
        KX         = 36,    // Key Exchanger                               [RFC2230]
        CERT       = 37,    // CERT                                        [RFC4398]
        A6         = 38,    // A6 (Experimental)                           [RFC3226][RFC2874]
        DNAME      = 39,    // DNAME                                       [RFC2672]
        SINK       = 40,    // SINK                                        [Eastlake]
        OPT        = 41,    // OPT                                         [RFC2671]
        APL        = 42,    // APL                                         [RFC3123]
        DS         = 43,    // Delegation Signer                           [RFC4034][RFC3658]
        SSHFP      = 44,    // SSH Key Fingerprint                         [RFC4255]
        IPSECKEY   = 45,    // IPSECKEY                                    [RFC4025]
        RRSIG      = 46,    // RRSIG                                       [RFC4034][RFC3755]
        NSEC       = 47,    // NSEC                                        [RFC4034][RFC3755]
        DNSKEY     = 48,    // DNSKEY                                      [RFC4034][RFC3755]
        DHCID      = 49,    // DHCID                                       [RFC4701]
        NSEC3      = 50,    // NSEC3                                       [RFC5155]
        NSEC3PARAM = 51,    // NSEC3PARAM                                  [RFC5155]
        HIP        = 55,    // Host Identity Protocol                      [RFC5205]
        NINFO      = 56,    // NINFO                                       [Reid]
        RKEY       = 57,    // RKEY                                        [Reid]
        SPF        = 99,    //                                             [RFC4408]
        UINFO      = 100,   //                                             [IANA-Reserved]
        UID        = 101,   //                                             [IANA-Reserved]
        GID        = 102,   //                                             [IANA-Reserved]
        UNSPEC     = 103,   //                                             [IANA-Reserved]
        TKEY       = 249,   // Transaction Key                             [RFC2930]
        TSIG       = 250,   // Transaction Signature                       [RFC2845]
        IXFR       = 251,   // incremental transfer                        [RFC1995]
        AXFR       = 252,   // transfer of an entire zone                  [RFC1035]
        MAILB      = 253,   // mailbox-related RRs (MB, MG or MR)          [RFC1035]
        MAILA      = 254,   // mail agent RRs (Obsolete - see MX)          [RFC1035]
        ANY        = 255,   // A request for all records (*)               [RFC1035]
        TA         = 32768, // DNSSEC Trust Authorities                    [Weiler] 2005-12-13
        DLV        = 32769, // DNSSEC Lookaside Validation                 [RFC4431]
        WINS       = 65281, // WINS records (WINS Lookup record)           [MS DNS]
        WINSR      = 65282  // WINSR records (WINS Reverse Lookup record)  [MS DNS]
    }

    public enum QR : ushort
    {
        Query     = 0,
        Response  = 32768
    }

    public enum OpCode : ushort
    {
        Query      = 0,     //                                             [RFC1035]
        IQuery     = 1,     //                                             [RFC3425]
        Status     = 2,     //                                             [RFC1035]
        Notify     = 4,     //                                             [RFC1996]
        Update     = 5      //                                             [RFC2136]
    }

    // A modification of the DNS flags to allow simple parsing of a 16-bit value

    [FlagsAttribute]
    public enum Flag : ushort
    {
        NONE = 0,
        AA = 1024,          // Authoritative Answer                        [RFC1035]
        TC = 512,           // Truncated Response                          [RFC1035]
        RD = 256,           // Recursion Desired                           [RFC1035]
        RA = 128,           // Recursion Allowed                           [RFC1035]
        AD = 32,            // Authenticated Data                          [RFC4035]
        CD = 16             // Checking Disabled                           [RFC4035]
    }

    public enum RCode : int
    {
        NoError    = 0,     // No Error                                    [RFC1035]
        FormErr    = 1,     // Format Error                                [RFC1035]
        ServFail   = 2,     // Server Failure                              [RFC1035]
        NXDomain   = 3,     // Non-Existent Domain                         [RFC1035]
        NotImp     = 4,     // Not Implemented                             [RFC1035]
        Refused    = 5,     // Query Refused                               [RFC1035]
        YXDomain   = 6,     // Name Exists when it should not              [RFC2136]
        YXRRSet    = 7,     // RR Set Exists when it should not            [RFC2136]
        NXRRSet    = 8,     // RR Set that should exist does not           [RFC2136]
        NotAuth    = 9,     // Server Not Authoritative for zone           [RFC2136]
        NotZone    = 10,    // Name not contained in zone                  [RFC2136]
        BadVers    = 16,    // Bad OPT Version                             [RFC2671]
        BadSig     = 16,    // TSIG Signature Failure                      [RFC2845]
        BadKey     = 17,    // Key not recognized                          [RFC2845]
        BadTime    = 18,    // Signature out of time window                [RFC2845]
        BadMode    = 19,    // Bad TKEY Mode                               [RFC2930]
        BadName    = 20,    // Duplicate key name                          [RFC2930]
        BadAlg     = 21,    // Algorithm not supported                     [RFC2930]
        BadTrunc   = 22     // Bad Truncation                              [RFC4635]
    }

    internal enum Compression : byte
    {
        Enabled    = 192,
        Disabled   = 0
    }

    public enum AFSDBSubType : ushort
    {
        AFSv3Loc   = 1,     // Andrews File Service v3.0 Location Service  [RFC1183]
        DCENCARoot = 2      // DCE/NCA root cell directory node            [RFC1183]
    }

    public enum ATMAFormat : ushort
    {
        AESA = 0,           // ATM End System Address
        E164 = 1,           // E.164 address format
        NSAP = 2            // Network Service Access Protocol (NSAP) address model 
    }

    public enum EDNSOption : int
    {
        LLQ        = 1,     // On-hold      [http://files.dns-sd.org/draft-sekar-dns-llq.txt]
        UL         = 2,     // On-hold      [http://files.dns-sd.org/draft-sekar-dns-ul.txt]
        NSID       = 3      // Standard                                    [RFC5001]
    }

    public enum WINSMappingFlag : uint
    {
        // http://msdn.microsoft.com/en-us/library/ms682748%28VS.85%29.aspx
        Replication   = 0,
        NoReplication = 65536
    }

    //public enum MSDNSOption : uint
    //{
    //    CompressXFR = 19795
    //}

    [FlagsAttribute]
    public enum EDnsFlag : ushort
    {
        NONE       = 0,
        DO         = 32768      // DNSSEC answer OK                            [RFC4035][RFC3225]
    }

    public enum EDnsOptionName : ushort
    {
        LLQ       = 1,          // On-hold                                     [http://files.dns-sd.org/draft-sekar-dns-llq.txt]
        UL        = 2,          // On-hold                                     [http://files.dns-sd.org/draft-sekar-dns-ul.txt]
        NSID      = 3           //                                             [RFC5001]
    }
}
