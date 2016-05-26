using System;
using System.Management;
using System.Net;

namespace DnsShell.Management
{
    public class SIG : ResourceRecord
    {
        public UInt16 TypeCovered { get; private set; }
        public UInt16 Algorithm { get; private set; }
        public UInt16 Labels { get; private set; }
        public UInt32 OriginalTTL { get; private set; }
        public UInt32 SignatureExpiration { get; private set; }
        public UInt32 SignatureInception { get; private set; }
        public UInt16 KeyTag { get; private set; }
        public String SignerName { get; private set; }
        public String Signature { get; private set; }

        internal SIG(ManagementObject wmiRecord, String ServerName)
        {
            this.UpdateProperties(wmiRecord);
            this.ServerName = ServerName;
            this.RecordType = RecordType.SIG;
            this.TypeCovered = (UInt16)wmiRecord.Properties["TypeCovered"].Value;
            this.Algorithm = (UInt16)wmiRecord.Properties["Algorithm"].Value;
            this.Labels = (UInt16)wmiRecord.Properties["Labels"].Value;
            this.OriginalTTL = (UInt32)wmiRecord.Properties["OriginalTTL"].Value;
            this.SignatureExpiration = (UInt32)wmiRecord.Properties["SignatureExpiration"].Value;
            this.SignatureInception = (UInt32)wmiRecord.Properties["SignatureInception"].Value;
            this.KeyTag = (UInt16)wmiRecord.Properties["KeyTag"].Value;
            this.SignerName = (String)wmiRecord.Properties["SignerName"].Value;
            this.Signature = (String)wmiRecord.Properties["Signature"].Value;
        }

        internal String Modify(
            UInt32 TTL = UInt32.MaxValue,
            UInt16 TypeCovered = UInt16.MaxValue,
            UInt16 Algorithm = UInt16.MaxValue,
            UInt16 Labels = UInt16.MaxValue,
            UInt32 OriginalTTL = UInt32.MaxValue,
            UInt32 SignatureExpiration = UInt32.MaxValue,
            UInt32 KeyTag = UInt32.MaxValue,
            String SignerName = "",
            String Signature = "")
        {
            ManagementBaseObject inParams = this.wmiResourceRecord.GetMethodParameters("Modify");

            if (TTL != UInt32.MaxValue & TTL != this.TTL)
            {
                inParams["TTL"] = TTL;
            }
            if (TypeCovered != UInt16.MaxValue & TypeCovered != this.TypeCovered)
            {
                inParams["TypeCovered"] = TypeCovered;
            }
            if (Algorithm != UInt16.MaxValue & Algorithm != this.Algorithm)
            {
                inParams["Algorithm"] = Algorithm;
            }
            if (Labels != UInt16.MaxValue & Labels != this.Labels)
            {
                inParams["Labels"] = Labels;
            }
            if (OriginalTTL != UInt32.MaxValue & OriginalTTL != this.OriginalTTL)
            {
                inParams["OriginalTTL"] = OriginalTTL;
            }
            if (SignatureExpiration != UInt32.MaxValue & SignatureExpiration != this.SignatureExpiration)
            {
                inParams["SignatureExpiration"] = SignatureExpiration;
            }
            if (KeyTag != UInt32.MaxValue & KeyTag != this.KeyTag)
            {
                inParams["KeyTag"] = KeyTag;
            }
            if (SignerName != String.Empty & SignerName != this.SignerName)
            {
                inParams["SignerName"] = SignerName;
            }
            if (Signature != String.Empty & Signature != this.Signature)
            {
                inParams["Signature"] = Signature;
            }

            ManagementBaseObject outParams = this.wmiResourceRecord.InvokeMethod("Modify", inParams, null);

            return (String)outParams["RR"];
        }
    }
}
