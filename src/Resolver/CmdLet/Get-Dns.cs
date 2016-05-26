using System;
using System.Collections;
using System.Diagnostics;
using System.Management.Automation;
using DnsShell.Resolver;
using System.Net;
using System.Net.Sockets;

namespace DnsShell.PowerShell.CmdLet
{
    [Cmdlet(
        VerbsCommon.Get, "Dns",
        DefaultParameterSetName = "Universal")]
    public class GetDns : Cmdlet
    {
        #region Parameter:Name (String) :: Parameter Set <All>
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "")]
        [ValidatePattern(@"^([A-Z0-9]|_[A-Z])(([\w\-]{0,61})[^_\-])?(\.([A-Z0-9]|_[A-Z])(([\w\-]{0,61})[^_\-])?)*$|^\.$")]
        public String Name = ".";
        #endregion

        #region Parameter:RecordType (RecordType) :: Parameter Set <All>
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName = "")]
        [Alias("Type")]
        public RecordType RecordType = RecordType.ANY;
        #endregion

        #region Parameter:Server (String) :: Parameter Set <All>
        [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "")]
        [Alias("ComputerName", "ServerName")]
        public String Server = String.Empty;
        #endregion

        #region Parameter:Port (UInt16) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public UInt16 Port = 53;
        #endregion

        #region Parameter:TimeOut (Byte) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        [ValidateRange(1, 30)]
        public Byte TimeOut = 5;
        #endregion
        
        #region Parameter:Tcp (Switch) :: Parameter Set <All>
        [Parameter(ParameterSetName = "")]
        public SwitchParameter Tcp;
        #endregion

        #region Parameter:RecordClass (RecordClass) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        [Alias("Class")]
        public RecordClass RecordClass = RecordClass.IN;
        #endregion

        #region Parameter:NoRecursion (Switch) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        public SwitchParameter NoRecursion;
        #endregion

        #region Parameter:SearchList (String[]) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        public String[] SearchList = new String[] { String.Empty };
        #endregion

        #region Parameter:AppendToMultiLabel (Switch) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        public SwitchParameter AppendToMultiLabel;
        #endregion

        #region Parameter:EDnsSize (UInt16) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        public UInt16 EDnsSize;
        #endregion

        #region Parameter:RequestNSID (Switch) :: Parameter Set 1
        [Parameter(ParameterSetName = "PS1")]
        public SwitchParameter RequestNSID;
        #endregion

        #region Parameter:Iterative (Switch) :: Parameter Set 2
        [Parameter(ParameterSetName = "PS2")]
        [Alias("Trace")]
        public SwitchParameter Iterative;
        #endregion

        #region Parameter:NSSearch (Switch) :: Parameter Set 3
        [Parameter(ParameterSetName = "PS3")]
        public SwitchParameter NSSearch;
        #endregion

        #region Parameter:ZoneTransfer (Switch) :: Parameter Set 4
        [Parameter(ParameterSetName = "PS4")]
        [Alias("List", "Transfer")]
        public SwitchParameter ZoneTransfer;
        #endregion

        #region Parameter:Incremental (UInt32) :: Parameter Set 4
        [Parameter(ParameterSetName = "PS4")]
        [Alias("SerialNumber")]
        public UInt32 Incremental = 0;
        #endregion

        private Int32 ResponseBufferSize = 4096;
        private ArrayList ResponseMessages = new ArrayList();

        protected override void ProcessRecord()
        {
            if (this.Server == String.Empty)
            {
                this.Server = (Utility.GetDnsServer())[0];
            }

            IPAddress CheckReverse;
            if (IPAddress.TryParse(Name, out CheckReverse))
            {
                String[] Temp = Name.Split('.');
                Array.Reverse(Temp);
                Name = String.Format("{0}.in-addr.arpa", String.Join(".", Temp));
            }

            if (this.ZoneTransfer | this.RecordType == RecordType.AXFR)
            {
                if (Incremental > 0)
                {
                    IXFRLookup(this.Name, this.Incremental, this.Server);
                }
                else
                {
                    this.Tcp = true;
                    Lookup(this.Name, RecordType.AXFR, this.Server, true);
                }
            }
            else if (!this.Name.Contains(".") | this.AppendToMultiLabel)
            {
                if (this.SearchList[0] == String.Empty)
                {
                    this.SearchList = Utility.GetSearchList();
                }

                LookupWithSuffix(this.Name, this.RecordType, this.Server);
            }
            else if (Iterative)
            {
                IterativeLookup(this.Name, this.RecordType, this.Server);
            }
            else if (NSSearch)
            {
                NsSearch(this.Name, this.Server);
            }
            else
            {
                Lookup(this.Name, this.RecordType, Server, true);
            }
        }

        //
        // Query methods
        //

        // Lookup
        //
        // A simple lookup

        internal void Lookup(String Name, RecordType RecordType, String Server, Boolean Display)
        {
            Boolean RecursionDesired = true;
            if (NoRecursion) { RecursionDesired = false; }

            Header DnsHeader = new Header(RecursionDesired, 1);
            if (EDnsSize > 0 | RequestNSID)
            {
                DnsHeader.AdditionalCount = 1;
            }
            
            Question DnsQuestion = new Question(Name, RecordType);

            Byte[] Payload = CreatePayload(DnsHeader, DnsQuestion);
            if (EDnsSize > 0 | RequestNSID)
            {
                UInt16 OptionSize = 0;
                if (EDnsSize == 0) { EDnsSize = 4096; }
                if (RequestNSID) { OptionSize = 4; }

                Int32 NewLength = Payload.Length + 11;
                if (RequestNSID) { NewLength += 4; }

                Byte[] PayloadWithEDns = new Byte[NewLength];

                Array.Copy(Payload, PayloadWithEDns, Payload.Length);
                Array.Copy(Resolver.EDns.ToByte(EDnsSize, OptionSize), 0, PayloadWithEDns, Payload.Length, 11);

                if (RequestNSID)
                {
                    Array.Copy(EDnsOption.NSIDOption(), 0, PayloadWithEDns, NewLength - 4, 4);
                }

                Payload = PayloadWithEDns;
            }

            // Clear previous responses
            ResponseMessages.Clear();
            ExecuteLookup(Payload, RecordType, Server, Display);
        }

        // Lookup with Suffix
        //
        // Append DNS Suffixes to a question

        internal void LookupWithSuffix(String Name, RecordType RecordType, String Server)
        {
            Boolean HasAnswer = false;
            for (int i = 0; i < SearchList.Length; i++)
            {
                String NameWithSuffix = String.Format("{0}.{1}", Name, SearchList[i]);
                Lookup(NameWithSuffix, RecordType, Server, true);

                if (((DnsPacket)ResponseMessages[0]).Header.RCode != RCode.NXDomain)
                {
                    HasAnswer = true;
                    break;
                }
            }
            if (!HasAnswer & Name.Contains("."))
            {
                Lookup(Name, RecordType, Server, true);
            }
        }

        // IterativeLookup
        //
        // Performs Iterative Name Resolution starting with the Root Servers.
        // Root Servers are taken from the locally configured DNS server or the specified DNS server.

        internal void IterativeLookup(String Name, RecordType RecordType, String Server)
        {
            Lookup(".", RecordType.NS, Server, false);
            DnsPacket Message = (DnsPacket)ResponseMessages[0];

            if (Message.Header.RCode == RCode.NoError)
            {
                do
                {
                    if (Message.Header.AdditionalCount > 0)
                    {
                        Server = (Message.Additional[0]).RecordData;
                    }
                    else if (Message.Header.AuthorityCount > 0)
                    {
                        Server = (Message.Authority[0]).RecordData;
                    }
                    ResponseMessages.Clear();

                    Header DnsHeader = new Header(false, 1);
                    Question DnsQuestion = new Question(Name, RecordType);

                    Byte[] Payload = CreatePayload(DnsHeader, DnsQuestion);

                    ExecuteLookup(Payload, RecordType, Server, true);
                    Message = (DnsPacket)ResponseMessages[0];
                } while (Message.Header.RCode == RCode.NoError & Message.Header.AnswerCount == 0);
            }
        }

        // NsSearch
        //
        // Executes a query for the SOA from the local or specified DNS server.
        // Executes a query for the NS records from the SOA.
        // Performs the query on each listed Name Server.

        internal void NsSearch(String Name, String Server)
        {
            String Domain = Name;

            Boolean HasSOA = false;
            Boolean NoError = true;
            do
            {
                Lookup(Domain, RecordType.SOA, Server, false);

                DnsPacket Message = (DnsPacket)ResponseMessages[0];
                if (Message.Header.RCode == RCode.NoError)
                {
                    foreach (ResourceRecord Answer in Message.Answer)
                    {
                        if (Answer.RecordType == RecordType.SOA)
                        {
                            SOA SOARecord = (SOA)Answer;
                            HasSOA = true;
                            Server = SOARecord.TargetName;
                        }
                    }
                    if (!HasSOA)
                    {
                        // Remove a label and retry.
                        if (Domain.Contains("."))
                        {
                            Domain = Domain.Substring(Domain.IndexOf('.') + 1, Domain.Length - Domain.IndexOf('.') - 1);
                        }
                        else
                        {
                            NoError = false;
                        }
                    }
                }
                else
                {
                    NoError = false;
                }
            } while (!HasSOA & NoError);

            // Get the list of name servers from the SOA
            if (HasSOA)
            {
                Lookup(Domain, RecordType.NS, Server, false);

                Object[] NameServers = ((DnsPacket)ResponseMessages[0]).Answer;

                // Get the answer for Name from each Authoritative Server
                foreach (ResourceRecord NameServer in NameServers)
                {
                    Server = NameServer.RecordData;
                    Lookup(Name, RecordType, Server, true);
                }
            }
        }

        // Incremental Zone Transfer
        //
        // Get the current SOA, modify the serial and send the request.

        internal void IXFRLookup(String Name, UInt32 Serial, String Server)
        {
            this.Tcp = false;
            Lookup(Name, RecordType.SOA, Server, false);

            // Use the server returned by the SOA as a target for the query
            DnsPacket Message = (DnsPacket)ResponseMessages[0];

            SOA SOARecord = new SOA(Name, Serial);

            Header DnsHeader = new Header(false, 1);
            DnsHeader.AuthorityCount = 1;
            Question DnsQuestion = new Question(Name, RecordType.IXFR);
            Byte[] Payload = CreatePayload(DnsHeader, DnsQuestion);

            Payload = AddAuthority(Payload, SOARecord);

            // Clear old responses
            ResponseMessages.Clear();
            this.Tcp = true;
            ExecuteLookup(Payload, RecordType.IXFR, Server, true);
        }

        //
        // Internal methods
        //

        internal Byte[] CreatePayload(Header DnsHeader, Question DnsQuestion)
        {
            Byte[] HeaderBytes = DnsHeader.ToByte();
            Byte[] QuestionBytes = DnsQuestion.ToByte();
            Byte[] Payload = new Byte[HeaderBytes.Length + QuestionBytes.Length];

            Array.Copy(HeaderBytes, Payload, HeaderBytes.Length);
            Array.Copy(QuestionBytes, 0, Payload, HeaderBytes.Length, QuestionBytes.Length);

            return Payload;
        }

        internal Byte[] AddAuthority(Byte[] Payload, SOA SOARecord)
        {
            Byte[] SOABytes = SOARecord.ToIxfrByte();

            Byte[] PayloadWithAuth = new Byte[Payload.Length + SOABytes.Length];

            Array.Copy(Payload, PayloadWithAuth, Payload.Length);
            Array.Copy(SOABytes, 0, PayloadWithAuth, Payload.Length, SOABytes.Length);

            return PayloadWithAuth;
        }

        internal void ExecuteLookup(Byte[] Payload, RecordType RecordType, String Server, Boolean DisplayResponse)
        {
            EndPoint RemoteEndPoint = this.CreateRemoteEndPoint(Server);
            Socket DnsClient = this.CreateSocket(RemoteEndPoint.AddressFamily);

            Byte[] ReceiveBuffer = new Byte[ResponseBufferSize];

            if (this.Tcp)
            {
                DateTime Start = DateTime.Now;

                Payload = PrefixLength(Payload);

                DnsClient.Connect(RemoteEndPoint);
                if (DnsClient.Connected)
                {
                    DnsClient.Send(Payload);
                }

                ArrayList ResponseBytes = new ArrayList();

                Boolean IsAXFR = false;
                Boolean IsIXFR = false; UInt32 Serial = 0; UInt32 SOACount = 0;
                Boolean Complete = false;

                do
                {
                    Boolean CanReadReply = true;
                    Int32 NumberOfBytesReceived = DnsClient.Receive(ReceiveBuffer);
                    
                    // Copy the received bytes into an ArrayList
                    Byte[] ReceivedBytes = new Byte[NumberOfBytesReceived];
                    Array.Copy(ReceiveBuffer, ReceivedBytes, NumberOfBytesReceived);

                    // Add the contents of this packet to ResponseBytes
                    ResponseBytes.AddRange(ReceivedBytes);

                    while (ResponseBytes.Count > 2 & CanReadReply)
                    {
                        Int32 ResponseLength = ((Int32)(Byte)ResponseBytes[0] << 8) | (Int32)(Byte)ResponseBytes[1];

                        if (ResponseBytes.Count >= ResponseLength + 2)
                        {
                            DnsPacket Message = new DnsPacket((Byte[])ResponseBytes.GetRange(2, ResponseLength).ToArray(typeof(Byte)));
                            Message.Server = Server;
                            Message.TimeTaken = (DateTime.Now - Start).TotalMilliseconds;

                            ResponseMessages.Add(Message);

                            if (DisplayResponse) { WriteObject(Message); }

                            ResponseBytes.RemoveRange(0, ResponseLength + 2);

                            if (Message.Header.RCode != RCode.NoError)
                            {
                                Complete = true;
                            }
                            else
                            {
                                switch (RecordType)
                                {
                                    case RecordType.IXFR:
                                        // See if the transfer type has switched to AXFR
                                        if (!IsIXFR & !IsAXFR)
                                        {
                                            if (ResponseMessages.Count == 1)
                                            {
                                                if (Message.Header.AnswerCount > 1)
                                                {
                                                    if ((Message.Answer[1]).RecordType == RecordType.SOA)
                                                    {
                                                        IsIXFR = true;
                                                        if (((SOA)Message.Answer[1]).Serial == Serial)
                                                        {
                                                            Complete = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        RecordType = RecordType.AXFR;
                                                        IsAXFR = true;
                                                    }
                                                }
                                            }
                                            else if (ResponseMessages.Count == 2)
                                            {
                                                if ((Message.Answer[0]).RecordType == RecordType.SOA)
                                                {
                                                    IsIXFR = true;
                                                }
                                                else
                                                {
                                                    RecordType = RecordType.AXFR;
                                                    IsAXFR = true;
                                                }
                                            }
                                        }

                                        if (Serial == 0 & ResponseMessages.Count == 1)
                                        {
                                            Serial = ((SOA)Message.Answer[0]).Serial;
                                            if (Incremental > Serial)
                                            {
                                                Complete = true;
                                            }
                                        }

                                        foreach (ResourceRecord Answer in Message.Answer)
                                        {
                                            if (Answer.RecordType == RecordType.SOA)
                                            {
                                                if (((SOA)Answer).Serial == Serial)
                                                {
                                                    SOACount++;
                                                }
                                            }
                                        }

                                        if (SOACount == 3 & IsIXFR) 
                                        {
                                            Complete = true;
                                        }
                                        if (SOACount == 2 & IsAXFR)
                                        {
                                            Complete = true;
                                        }
                                        break;
                                    case RecordType.AXFR:
                                        if (ResponseMessages.Count == 1 & Message.Header.AnswerCount > 1)
                                        {
                                            if ((Message.Answer[Message.Header.AnswerCount - 1]).RecordType == RecordType.SOA)
                                            {
                                                Complete = true;
                                            }
                                        }
                                        else if (ResponseMessages.Count > 1)
                                        {
                                            if ((Message.Answer[Message.Header.AnswerCount - 1]).RecordType == RecordType.SOA)
                                            {
                                                Complete = true;
                                            }
                                        }

                                        break;
                                    default:
                                        Complete = true;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            CanReadReply = false;
                        }
                    }
                } while (!Complete);
            }
            else
            {
                DateTime Start = DateTime.Now;
                DnsClient.SendTo(Payload, RemoteEndPoint);

                EndPoint Sender = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                Int32 NumberOfBytesReceived = DnsClient.ReceiveFrom(ReceiveBuffer, ref Sender);
                Byte[] Response = new Byte[NumberOfBytesReceived];
                Array.Copy(ReceiveBuffer, Response, NumberOfBytesReceived);
                
                DnsPacket Message = new DnsPacket(Response);
                Message.Server = Server;
                Message.TimeTaken = (DateTime.Now - Start).TotalMilliseconds;

                ResponseMessages.Add(Message);

                if (DisplayResponse) { WriteObject(Message); }
            }

            this.CloseSocket(DnsClient);
        }

        //
        // Private methods
        //

        private Socket CreateSocket(AddressFamily AddressFamily)
        {
            SocketType SocketType = SocketType.Dgram;
            ProtocolType ProtocolType = ProtocolType.Udp;
            if (this.Tcp)
            { 
                SocketType = SocketType.Stream; 
                ProtocolType = ProtocolType.Tcp;
            }

            Socket DnsClient = new Socket(AddressFamily, SocketType, ProtocolType);
            DnsClient.ReceiveTimeout = TimeOut * 1000;

            return DnsClient;
        }

        private EndPoint CreateRemoteEndPoint(String Server)
        {
            IPAddress RemoteIP;
            // Check the value used for the server address
            if (!(IPAddress.TryParse(Server, out RemoteIP)))
            {
                IPHostEntry ServerInfo = Dns.GetHostEntry(Server);
                RemoteIP = ServerInfo.AddressList[0];
            }

            return (EndPoint)(new IPEndPoint(RemoteIP, Port));
        }

        private Byte[] PrefixLength(Byte[] Payload)
        {
            Byte[] TcpBytes = new Byte[Payload.Length + 2];
            Array.Copy(EndianBitConverter.ToByte((UInt16)Payload.Length, false), 0, TcpBytes, 0, 2);
            Array.Copy(Payload, 0, TcpBytes, 2, Payload.Length);

            return TcpBytes;
        }

        private void CloseSocket(Socket DnsClient)
        {
            DnsClient.Shutdown(SocketShutdown.Both);
            DnsClient.Close();
        }
    }
}