using System;
using System.Management;
using System.Management.Automation;
using System.Security;
using System.Text.RegularExpressions;
using DnsShell.Management;

namespace DnsShell.PowerShell.CmdLet
{
    public class ManagementCmdlet : PSCmdlet
    {
        //
        // Common Parameters
        //

        #region Parameter:Credential (PSCredential)
        [Parameter(ParameterSetName = "")]
        public PSCredential Credential;
        #endregion

        //
        // Common Properties
        //

        private String Namespace = @"root\MicrosoftDNS";

        private ConnectionOptions Options;

        private ManagementScope Scope;
        private ManagementPath Path;
        private ManagementClass Registry;
        private String WMIServerName;
        internal UInt32 ErrorLevel = 0;

        //
        // Parameter Validators
        //

        internal Boolean IsValidIPAddress(String IPAddress)
        {
            System.Net.IPAddress IPAddr;
            if (System.Net.IPAddress.TryParse(IPAddress, out IPAddr))
            {
                if (IPAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return true;
                }
            }
            return false;
        }

        internal Boolean IsValidIPv6Address(String IPAddress)
        {
            System.Net.IPAddress IPAddr;
            if (System.Net.IPAddress.TryParse(IPAddress, out IPAddr))
            {
                if (IPAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return true;
                }
            }
            return false;
        }

        //
        // Common Methods
        //

        internal void SetOptions()
        {
            this.Options = new ConnectionOptions();
            if (this.Credential != null)
            {
                this.Options.Username = Credential.UserName;
                this.Options.SecurePassword = Credential.Password;
            }
        }

        internal void SetScope(String ServerName)
        {
            this.Scope = new ManagementScope(String.Format(@"\\{0}\{1}", ServerName, this.Namespace), this.Options);
            this.WMIServerName = ServerName;
        }

        internal void SetManagementPath(String Path)
        {
            this.Path = new ManagementPath(Path);
        }

        internal ManagementObjectCollection Search(String Filter)
        {
            String QueryString = String.Format("SELECT * FROM {0}", this.Path.Path);

            if (Filter != String.Empty)
            {
                QueryString = String.Format("{0} WHERE {1}", QueryString, Filter);
            }
            ObjectQuery Query = new ObjectQuery(QueryString);

            ManagementObjectSearcher wmiSearch = new ManagementObjectSearcher(
                Scope,
                Query);

            ManagementObjectCollection wmiResults;
            try
            {
                wmiResults = wmiSearch.Get();
                return wmiResults;
            }
            catch (ManagementException e)
            {
                ErrorLevel = 1;
                WriteError(new ErrorRecord(
                    e,
                    "WMIManagementException",
                    ErrorCategory.InvalidOperation,
                    typeof(ManagementCmdlet)));
            }
            catch (UnauthorizedAccessException e)
            {
                ErrorLevel = 1;
                WriteError(new ErrorRecord(
                    e,
                    "UnauthorizedAccessException",
                    ErrorCategory.PermissionDenied,
                    typeof(ManagementCmdlet)));
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                ErrorLevel = 1;
                WriteError(new ErrorRecord(
                    e,
                    "RPCServerUnavailable",
                    ErrorCategory.ResourceUnavailable,
                    typeof(ManagementCmdlet)));
            }
            return null;
        }

        internal ManagementObject Get()
        {
            ManagementObject wmiObject = new ManagementObject(
                this.Scope,
                this.Path,
                new ObjectGetOptions());

            return wmiObject;
        }

        internal ManagementClass GetClass()
        {
            ManagementClass wmiClass = new ManagementClass(
                this.Scope,
                this.Path,
                new ObjectGetOptions());

            return wmiClass;
        }

        //
        // Registry
        //

        internal void ConnectRegistry(String ServerName)
        {
            ManagementScope RegistryScope = new ManagementScope(
                String.Format(@"//{0}/root/default", ServerName), this.Options);

            this.Registry = new ManagementClass(RegistryScope, new ManagementPath("StdRegProv"), null);
        }

        private ManagementBaseObject GetValue(
            String GetValueMethod,
            String Key,
            String ValueName,
            RegistryHive Hive)
        {
            ManagementBaseObject inParams = this.Registry.GetMethodParameters(GetValueMethod);
            inParams["hDefKey"] = Hive;
            inParams["sSubKeyName"] = Key;
            inParams["sValueName"] = ValueName;

            return this.Registry.InvokeMethod(GetValueMethod, inParams, null);
        }

        internal UInt32 GetDWordValue(
            String Key,
            String ValueName,
            RegistryHive Hive = RegistryHive.HKLM)
        {
            ManagementBaseObject outParams = this.GetValue("GetDWORDValue", Key, ValueName, Hive);

            if (outParams["uValue"] == null)
            {
                return UInt32.MaxValue;
            }
            return (UInt32)outParams["uValue"];
        }

        internal String[] GetMultiStringValue(
            String Key,
            String ValueName,
            RegistryHive Hive = RegistryHive.HKLM)
        {
            ManagementBaseObject outParams = this.GetValue("GetMultiStringValue", Key, ValueName, Hive);

            if (outParams["sValue"] == null)
            {
                return new String[] { String.Empty };
            }
            return (String[])outParams["sValue"];
        }

        //
        // Presentation
        //

        internal void WriteRecord(ManagementObject wmiRecord)
        {
            WmiRecordClass WMIRecordClass = 
                (WmiRecordClass)Enum.Parse(typeof(WmiRecordClass), (String)wmiRecord.Properties["__CLASS"].Value, true);

             switch (WMIRecordClass)
            {
                case WmiRecordClass.MicrosoftDNS_AType:
                    WriteObject(new A(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_AAAAType:
                    WriteObject(new AAAA(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_AFSDBType:
                    WriteObject(new AFSDB(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_ATMAType:
                    WriteObject(new ATMA(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_CNAMEType:
                    WriteObject(new CNAME(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_HINFOType:
                    WriteObject(new HINFO(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_ISDNType:
                    WriteObject(new ISDN(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_KEYType:
                    WriteObject(new KEY(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MBType:
                    WriteObject(new MB(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MDType:
                    WriteObject(new MD(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MFType:
                    WriteObject(new MF(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MGType:
                    WriteObject(new MG(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MINFOType:
                    WriteObject(new MINFO(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MRType:
                    WriteObject(new MR(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_MXType:
                    WriteObject(new MX(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_NSType:
                    WriteObject(new NS(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_NXTType:
                    WriteObject(new NXT(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_PTRType:
                    WriteObject(new PTR(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_RPType:
                    WriteObject(new RP(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_RTType:
                    WriteObject(new RT(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_SIGType:
                    WriteObject(new SIG(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_SOAType:
                    WriteObject(new SOA(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_SRVType:
                    WriteObject(new SRV(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_TXTType:
                    WriteObject(new TXT(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_WINSType:
                    WriteObject(new WINS(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_WINSRType:
                    WriteObject(new WINSR(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_WKSType:
                    WriteObject(new WKS(wmiRecord, this.WMIServerName));
                    break;
                case WmiRecordClass.MicrosoftDNS_X25Type:
                    WriteObject(new X25(wmiRecord, this.WMIServerName));
                    break;
            }
        }
    }
}
