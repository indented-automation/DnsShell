namespace DnsShell
{
    public enum ZonePropertyID : uint
    {
        Type = 1,                   // The zone type. See dwZoneType (section 2.2.5.2.4.1).
        AllowUpdate = 2,            // Whether dynamic updates are allowed. See fAllowUpdate (section 2.2.5.2.4.1).
        Securetime = 8,             // The time, in seconds and expressed as an unsigned 64-bit integer, at which the zone became secure.
        NoRefreshInterval = 16,     // The zone no refresh interval. See dwNoRefreshInterval (section 2.2.5.2.4.1).
        ScavengingServers = 17,     // Servers that will perform scavenging. See aipScavengingServers (section 2.2.5.2.4.1).
        AgingEnabledTime = 18,      // The time interval before the next scavenging cycle. See dwAvailForScavengeTime (section 2.2.5.2.4.1).
        RefreshInterval = 32,       // The zone refresh interval. See dwRefreshInterval (section 2.2.5.2.4.1).
        AgingState = 64,            // Whether aging is enabled. See fAging (section 2.2.5.2.4.1).
        DeletedFromHostname = 128,  // The name of the server that deleted the zone. The value is a null-terminated Unicode string.
        MasterServers = 129,        // Servers to perform zone transfers. See aipMasters (section 2.2.5.2.4.1).
        AutoNSServers = 130,        // A list of servers which MAY autocreate a delegation. The list is formatted as DNS_ADDR_ARRAY (section 2.2.3.2.3).
        DCPromoConvert = 131,       // The flag value representing the state of conversion of the zone. See DcPromo Flag (section 2.3.1.1.2).
        ScavengingServersDA = 144,  // Servers that will perform scavenging. Same format as DSPROPERTY_ZONE_SCAVENGING_SERVERS.
        MasterServersDA = 145,      // Servers to perform zone transfers. Same format as DSPROPERTY_ZONE_MASTER_SERVERS.
        AutoNSServersDA = 146,      // A list of servers which MAY autocreate a delegation. Same format as DSPROPERTY_ZONE_AUTO_NS_SERVERS.
        NodeDBFlags = 256           // See DNS_RPC_NODE_FLAGS (section 2.2.2.1.2).
    }

    public enum DcPromoFlag : uint
    {
        None = 0,                   // No change to existing zone storage.
        ConvertDomain = 1,          // Zone is to be stored in DNS domain partition. See DNS_ZONE_CREATE_FOR_DCPROMO (section 2.2.5.2.7.1).
        ConvertForest = 2           // Zone is to be stored in DNS forest partition. See DNS_ZONE_CREATE_FOR_DCPROMO_FOREST (section 2.2.5.2.7.1).
    }

    public enum Rank : byte
    {
        CacheBit = 1,               // The record came from the cache.
        RootHint = 8,               // The record is a preconfigured root hint.
        OutsideGlue = 32,           // This value is not used.
        CacheNAAdditional = 49,     // The record was cached from the additional section of a nonauthoritative response.
        CacheNAAuthority = 65,      // The record was cached from the authority section of a nonauthoritative response.
        CacheAAdditional = 81,      // The record was cached from the additional section of an authoritative response.
        CacheNAAnswer = 97,         // The record was cached from the answer section of a nonauthoritative response.
        CacheAAuthority = 113,      // The record was cached from the authority section of an authoritative response.
        Glue = 128,                 // The record is a glue record in an authoritative zone.
        NSGlue = 130,               // The record is a delegation (type NS) record in an authoritative zone.
        CacheAAnswer = 193,         // The record was cached from the answer section of an authoritative response.
        Zone = 240                  // The record comes from an authoritative zone.
    }
}
