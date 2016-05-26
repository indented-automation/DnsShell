using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DnsShell.Resolver
{
    // DnsShell.Resolver.Query - Contains methods to send and receive queries using UDP or TCP.
    //
    // Constructors:
    //   Query(Header DnsHeader, Question DnsQuestion, Settings DnsSettings) - Public
    //
    // Methods:
    //   UdpQuery - Protected
    //   TcpQuery - Protected
    //
    // To Do:
    //   UdpQuery - Graceful Try and Catch for UDP "connection"
    //   TcpQuery - Graceful Try and Catch for TCP connection

    public class Query
    {
        public Header Header;
        public Question Question;
        public PacketReader[] Reply;

        public Byte[] Bytes;
        public Byte TimeOut = 5;
        public IPEndPoint RemoteEndPoint;
        public ProtocolType ProtocolType
        {
            set
            {
                switch (value)
                {
                    case ProtocolType.Udp: this._SocketType = SocketType.Dgram; this._ProtocolType = value; break;
                    case ProtocolType.Tcp: this._SocketType = SocketType.Stream; this._ProtocolType = value; break;
                    default: throw new ArgumentException();
                }
            }
            get { return _ProtocolType; }
        }

        private Socket Client;
        private ProtocolType _ProtocolType = ProtocolType.Udp;
        private SocketType _SocketType = SocketType.Dgram;

        private Byte[] ReceiveBuffer = new Byte[512];

        private ArrayList ReplyArray = new ArrayList();

        // Constructors

        public Query() { }

        public Query(Header DnsHeader, Question DnsQuestion)
        {
            this.SetPayload(DnsHeader, DnsQuestion);
        }

        // Public Methods

        public void SetPayload(Header DnsHeader, Question DnsQuestion)
        {
            this.Header = DnsHeader;
            this.Question = DnsQuestion;

            Byte[] HeaderBytes = DnsHeader.ToByte();
            Byte[] QuestionBytes = DnsQuestion.ToByte();

            Byte[] Payload = new Byte[HeaderBytes.Length + QuestionBytes.Length];

            Array.Copy(HeaderBytes, Payload, HeaderBytes.Length);
            Array.Copy(QuestionBytes, 0, Payload, HeaderBytes.Length, QuestionBytes.Length);

            this.Bytes = Payload;
        }

        public void AddAuthority(ResourceRecord SOARecord)
        {
            Byte[] Payload = this.Bytes;
            Byte[] SOABytes = SOARecord.ToByte();

            Byte[] PayloadWithAuth = new Byte[Payload.Length + SOABytes.Length];

            Array.Copy(Payload, PayloadWithAuth, Payload.Length);
            Array.Copy(SOABytes, 0, PayloadWithAuth, Payload.Length, SOABytes.Length);

            this.Bytes = PayloadWithAuth;
        }

        public void SetEndPoint(String Server, UInt16 Port)
        {
            IPAddress RemoteIP;
            // Check the value used for the server address
            if (!(IPAddress.TryParse(Server, out RemoteIP)))
            {
                // Needs Try here
                IPHostEntry ServerInfo = Dns.GetHostEntry(Server);
                RemoteIP = ServerInfo.AddressList[0];
            }
            this.RemoteEndPoint = new IPEndPoint(RemoteIP, Port);
        }

        public void Send()
        {
            this.CreateSocket();

            ArrayList Responses = new ArrayList();
            switch (_ProtocolType)
            {
                case ProtocolType.Udp:
                    this.SendUdp();
                    this.ReceiveUdp();
                    break;
                case ProtocolType.Tcp:
                    this.SendTcp();
                    this.ReceiveTcp();
                    break;
            }

            this.CloseSocket();

            this.Reply = (PacketReader[])this.ReplyArray.ToArray(typeof(PacketReader));
        }

        // Private Methods

        private void CreateSocket()
        {
            this.Client = new Socket(AddressFamily.InterNetwork, this._SocketType, this._ProtocolType);
            this.Client.ReceiveTimeout = TimeOut * 1000;
        }

        private void SendUdp()
        {
            Client.SendTo(Bytes, (EndPoint)RemoteEndPoint);
        }

        private void ReceiveUdp()
        {
            EndPoint Sender = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
            Int32 NumberOfBytesReceived = Client.ReceiveFrom(ReceiveBuffer, ref Sender);
            Byte[] Response = new Byte[NumberOfBytesReceived];
            Array.Copy(ReceiveBuffer, Response, NumberOfBytesReceived);
            ReplyArray.Add(new PacketReader(Response));
        }

        private void SendTcp()
        {
            // Modify bytes, prefix the 2 byte length.
            Int32 Length = this.Bytes.Length + 2;

            Byte[] TcpBytes = new Byte[Length];
            Array.Copy(EndianBitConverter.ToByte((UInt16)this.Bytes.Length, false), 0, TcpBytes, 0, 2);
            Array.Copy(this.Bytes, 0, TcpBytes, 2, this.Bytes.Length);

            Client.Connect((EndPoint)RemoteEndPoint);
            if (Client.Connected)
            {
                Client.Send(TcpBytes);
            }
        }

        public ArrayList Response = new ArrayList();
        public string EndReason;

        private void ReceiveTcp()
        {
            Boolean Complete = false;
            do
            {
                Int32 NumberOfBytesReceived = Client.Receive(ReceiveBuffer);

                // Copy the received bytes into an ArrayList - Should be able to drop this bit.
                Byte[] ReceivedBytes = new Byte[NumberOfBytesReceived];
                Array.Copy(this.ReceiveBuffer, ReceivedBytes, NumberOfBytesReceived);

                // Add the contents of this packet to the Response
                Response.AddRange(ReceivedBytes);

                Boolean CanReadReply = true;
                while (Response.Count > 2 & CanReadReply)
                {
                    Int32 Length = ((Int32)(Byte)Response[0] << 8) | (Int32)(Byte)Response[1];

                    if (Response.Count >= Length + 2)
                    {
                        PacketReader Message = new PacketReader((Byte[])Response.GetRange(2, Length).ToArray(typeof(Byte)));
                        ReplyArray.Add(Message);
                        Response.RemoveRange(0, Length + 2);
                    }
                    else
                    {
                        CanReadReply = false;
                    }
                }

                // Connection termination
                if (Response.Count == 0)
                {
                    // On error
                    if (((PacketReader)ReplyArray[ReplyArray.Count - 1]).Header.RCode != "NoError")
                    {
                        EndReason = "RCode notified failure";
                        Complete = true;
                    }
                    // IXFR
                    else if (Question.RRType == "AXFR" | Question.RRType == "IXFR")
                    {

                        //                        receive the first response message;
                        //extract the first response RR, always an SOA;
                        //if (the serial number of this SOA RR is less
                        //    than or equal to that of the request) {
                        //    the response is an up-to-date response;
                        //} else {
                        //    if (the response message was received by UDP and
                        //        contains no more RRs after the initial SOA) {
                        //        the response is a UDP-overflow response;
                        //    } else {
                        //        extract the second response RR, waiting for a second TCP
                        //        response message if necessary;
                        //        if (this second RR is an SOA) {
                        //            the response is an incremental transfer;
                        //        } else {
                        //            the response is a nonincremental transfer;
                        //        }
                        //    }
                        //}




                    }
                    // AXFR - Must consist of at least two SOA records
                    else if (Question.RRType == "AXFR")
                    {
                        if (((PacketReader)ReplyArray[ReplyArray.Count - 1]).Answer[0].RRType == "SOA" &
                            ReplyArray.Count >= 2)
                        {
                            EndReason = "Complete AXFR";
                            Complete = true;
                        }
                    }
                    // Single query - one packet anyway
                    else if (Question.RRType != "AXFR" & Question.RRType != "IXFR")
                    {
                        EndReason = "Single answer expected";
                        Complete = true;
                    }
                }
            } while (!Complete);
        }

        private void CloseSocket()
        {
            Client.Shutdown(SocketShutdown.Both);
            Client.Close();
        }
    }
}
