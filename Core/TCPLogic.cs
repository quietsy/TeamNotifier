using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using CommonClassLibs;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TeamNotifier
{
    public class NewMessage : EventArgs
    {
        public string message;
        public int num1;
        public int num2;

        public NewMessage(string Message, int Num1 = 0, int Num2 = 0)
        {
            message = Message;
            num1 = Num1;
            num2 = Num2;
        }
    }

    public class TCPLogic
    {
        private Client client = null;

        private MotherOfRawPackets HostServerRawPackets = null;
        static AutoResetEvent autoEventHostServer = null;//mutex
        static AutoResetEvent autoEvent2;//mutex
        private Thread DataProcessHostServerThread = null;
        private Thread FullPacketDataProcessThread = null;
        private Queue<FullPacket> FullHostServerPacketList = null;
        bool AppIsExiting = false;
        bool ServerConnected = false;
        int MyHostServerID = 0;
        long ServerTime = DateTime.Now.Ticks;
        System.Windows.Threading.DispatcherTimer GeneralTimer = null;

        string server, user, room;
        int port;

        public TCPLogic()
        {
        }

        ~TCPLogic()
        {
            Disconnect();
        }
        
        public void ConnectToServer(string Server, int Port, string User, string Room)
        {
            try
            {
                server = Server;
                port = Port;
                user = User;
                room = Room;

                ServerConnected = true;
                InitializeServerConnection();
                if (ConnectToHostServer())
                {
                    ServerConnected = true;
                    ConnectionStatusEvent(this, EventArgs.Empty, true);
                    BeginGeneralTimer();
                }
                else
                {
                    ServerConnected = false;
                    ConnectionStatusEvent(this, EventArgs.Empty, false);
                }
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        private void BeginGeneralTimer()
        {
            if (GeneralTimer == null)
            {
                GeneralTimer = new System.Windows.Threading.DispatcherTimer();
                GeneralTimer.Tick += GeneralTimer_Tick;
                GeneralTimer.Interval = new TimeSpan(0, 0, 5);
                GeneralTimer.Start();
            }
        }

        private void GeneralTimer_Tick(object sender, EventArgs e)
        {
            if (client != null)
            {
                TimeSpan ts = DateTime.Now - client.LastDataFromServer;
                
                if (ts.TotalMinutes > 5)
                {
                    DestroyGeneralTimer();
                    HostCommunicationsHasQuit(false);
                }
            }
            
            ServerTime += (TimeSpan.TicksPerSecond * 5);
        }

        private void OnDisconnect()
        {
            DoServerDisconnect();
        }

        private void OnDataReceive(byte[] message, int messageSize)
        {
            if (AppIsExiting)
                return;

            HostServerRawPackets.AddToList(message, messageSize);
            if (autoEventHostServer != null)
                autoEventHostServer.Set();
        }

        private bool ConnectToHostServer()
        {
            try
            {
                if (client == null)
                {
                    client = new Client();
                    client.OnDisconnected += OnDisconnect;
                    client.OnReceiveData += OnDataReceive;
                }
                else
                {
                    if (client.Connected)
                        return true;
                }

                string szIPstr = GetSHubAddress();
                if (szIPstr.Length == 0)
                {
                    return false;
                }

                IPAddress ipAdd = IPAddress.Parse(szIPstr);
                client.Connect(ipAdd, port);

                if (client.Connected)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
            return false;
        }

        public void Disconnect()
        {
            try
            {
                TellServerImDisconnecting();
                DoServerDisconnect();

                if (ServerConnected)
                {
                    ServerConnected = false;
                    ConnectionStatusEvent(this, EventArgs.Empty, false);
                }
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        public void SendDataToServer(string user, string msg, int num1 = 0, int num2 = 0)
        {
            PACKET_DATA xdata = new PACKET_DATA();
            
            xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_Message;
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageStart;
            xdata.Packet_Size = 16;
            xdata.maskTo = 0;
            xdata.idTo = 0;
            xdata.idFrom = 0;
            
            xdata.Data16 = num1;
            xdata.Data17 = num2;

            int pos = 0;
            int chunkSize = xdata.szStringDataA.Length;

            if (msg.Length <= xdata.szStringDataA.Length)
            {
                msg.CopyTo(0, xdata.szStringDataA, 0, msg.Length);
                chunkSize = msg.Length;
            }
            else
                msg.CopyTo(0, xdata.szStringDataA, 0, xdata.szStringDataA.Length);

            if (!string.IsNullOrEmpty(room))
                room.CopyTo(0, xdata.szStringDataC, 0, room.Length);

            if (!string.IsNullOrEmpty(user))
                user.CopyTo(0, xdata.szStringData150, 0, user.Length);

            xdata.Data1 = (UInt32)chunkSize;

            byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

            SendMessageToServer(byData);
            
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageGuts;
            pos = chunkSize;
            
            xdata.Data_Type = (UInt16)PACKETTYPES_SUBMESSAGE.SUBMSG_MessageEnd;
            xdata.Data1 = (UInt32)pos;
            byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);
            SendMessageToServer(byData);
        }

        private void InitializeServerConnection()
        {
            try
            {
                autoEventHostServer = new AutoResetEvent(false);
                autoEvent2 = new AutoResetEvent(false);
                FullPacketDataProcessThread = new Thread(new ThreadStart(ProcessRecievedServerData));
                DataProcessHostServerThread = new Thread(new ThreadStart(NormalizeServerRawPackets));


                if (HostServerRawPackets == null)
                    HostServerRawPackets = new MotherOfRawPackets(0);
                else
                {
                    HostServerRawPackets.ClearList();
                }

                if (FullHostServerPacketList == null)
                    FullHostServerPacketList = new Queue<FullPacket>();
                else
                {
                    lock (FullHostServerPacketList)
                        FullHostServerPacketList.Clear();
                }

                FullPacketDataProcessThread.Start();
                DataProcessHostServerThread.Start();

            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        private string GetSHubAddress()
        {
            string SHubServer = server; 

            if (SHubServer.Length < 1)
                return string.Empty;

            try
            {
                string[] qaudNums = SHubServer.Split('.');
                
                if (qaudNums.Length != 4)
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(SHubServer);

                    foreach (IPAddress a in hostEntry.AddressList)
                    {
                        if (a.AddressFamily == AddressFamily.InterNetwork)
                        {
                            SHubServer = a.ToString();
                            break;
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                Log.Message(se.ToString());
            }

            return SHubServer;
        }
        private void NormalizeServerRawPackets()
        {
            try
            {
                while (ServerConnected)
                {
                    autoEventHostServer.WaitOne(10000);

                    if (AppIsExiting)
                        break;
                    

                    if (HostServerRawPackets.GetItemCount == 0)
                        continue;
                    
                    byte[] packetplayground = new byte[11264];
                    RawPackets rp;

                    int actualPackets = 0;

                    while (true)
                    {
                        if (HostServerRawPackets.GetItemCount == 0)
                            break;

                        int holdLen = 0;

                        if (HostServerRawPackets.bytesRemaining > 0)
                            Array.Copy(HostServerRawPackets.Remainder, 0, packetplayground, 0, HostServerRawPackets.bytesRemaining);

                        holdLen = HostServerRawPackets.bytesRemaining;

                        for (int i = 0; i < 10; i++)
                        {
                            rp = HostServerRawPackets.GetTopItem;

                            Array.Copy(rp.dataChunk, 0, packetplayground, holdLen, rp.iChunkLen);

                            holdLen += rp.iChunkLen;

                            if (HostServerRawPackets.GetItemCount == 0)
                                break;
                        }

                        actualPackets = 0;
                        
                        if (holdLen >= 1024)
                        {
                            actualPackets = holdLen / 1024;
                            HostServerRawPackets.bytesRemaining = holdLen - (actualPackets * 1024);

                            for (int i = 0; i < actualPackets; i++)
                            {
                                byte[] tmpByteArr = new byte[1024];
                                Array.Copy(packetplayground, i * 1024, tmpByteArr, 0, 1024);
                                lock (FullHostServerPacketList)
                                    FullHostServerPacketList.Enqueue(new FullPacket(HostServerRawPackets.iListClientID, tmpByteArr));
                            }
                        }
                        else
                        {
                            HostServerRawPackets.bytesRemaining = holdLen;
                        }
                        
                        Array.Copy(packetplayground, actualPackets * 1024, HostServerRawPackets.Remainder, 0, HostServerRawPackets.bytesRemaining);
                        
                        if (FullHostServerPacketList.Count > 0)
                            autoEvent2.Set();

                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }
        
        private void ReplyToHostCredentialRequest(byte[] message)
        {
            if (client == null)
                return;
            
            try
            {
                UInt16 PaketType = (UInt16)PACKETTYPES.TYPE_CredentialsUpdate;

                if (message != null)
                {
                    int myOldServerID = 0;
                    PACKET_DATA IncomingData = new PACKET_DATA();
                    IncomingData = (PACKET_DATA)PACKET_FUNCTIONS.ByteArrayToStructure(message, typeof(PACKET_DATA));

                    if (MyHostServerID > 0)
                        myOldServerID = MyHostServerID;

                    MyHostServerID = (int)IncomingData.idTo;


                    Log.Message($"My Host Server ID is {MyHostServerID}");

                    string MyAddressAsSeenByTheHost = new string(IncomingData.szStringDataA).TrimEnd('\0');

                    ServerTime = IncomingData.DataLong1;

                    PaketType = (UInt16)PACKETTYPES.TYPE_MyCredentials;
                }
                
                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = PaketType;
                xdata.Data_Type = 0;
                xdata.Packet_Size = (UInt16)Marshal.SizeOf(typeof(PACKET_DATA));
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;
                
                string p = Environment.MachineName;
                if (p.Length > (xdata.szStringDataA.Length - 1))
                    p.CopyTo(0, xdata.szStringDataA, 0, (xdata.szStringDataA.Length - 1));
                else
                    p.CopyTo(0, xdata.szStringDataA, 0, p.Length);
                xdata.szStringDataA[(xdata.szStringDataA.Length - 1)] = '\0';
                
                string VersionNumber = string.Empty;

                VersionNumber = Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." +
                                    Assembly.GetEntryAssembly().GetName().Version.Minor.ToString() + "." +
                                    Assembly.GetEntryAssembly().GetName().Version.Build.ToString();


                VersionNumber.CopyTo(0, xdata.szStringDataB, 0, VersionNumber.Length);
                
                if (room != null)
                    room.CopyTo(0, xdata.szStringDataC, 0, room.Length);
                
                string L = user;
                if (L.Length > (xdata.szStringData150.Length - 1))
                    L.CopyTo(0, xdata.szStringData150, 0, (xdata.szStringData150.Length - 1));
                else
                    L.CopyTo(0, xdata.szStringData150, 0, L.Length);
                xdata.szStringData150[(xdata.szStringData150.Length - 1)] = '\0';
                
                
                xdata.nAppLevel = (UInt16)APPLEVEL.None;


                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

                SendMessageToServer(byData);
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        private void SendMessageToServer(byte[] byData)
        {
            if (client != null && client.Connected)
                client.SendMessage(byData);
        }

        private void ProcessRecievedServerData()
        {
            try
            {
                while (ServerConnected)
                {
                    autoEvent2.WaitOne(10000);
                    if (AppIsExiting || !ServerConnected)
                        break;

                    while (FullHostServerPacketList.Count > 0)
                    {
                        try
                        {
                            FullPacket fp;
                            lock (FullHostServerPacketList)
                                fp = FullHostServerPacketList.Dequeue();

                            UInt16 type = (ushort)(fp.ThePacket[1] << 8 | fp.ThePacket[0]);
                            switch (type)
                            {
                                case (Byte)PACKETTYPES.TYPE_RequestCredentials:
                                    {
                                        ReplyToHostCredentialRequest(fp.ThePacket);
                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_Ping:
                                    {
                                        ReplyToHostPing(fp.ThePacket);
                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_HostExiting:
                                    HostCommunicationsHasQuit(true);
                                    break;
                                case (Byte)PACKETTYPES.TYPE_Registered:
                                    {

                                    }
                                    break;
                                case (Byte)PACKETTYPES.TYPE_MessageReceived:
                                    break;
                                case (Byte)PACKETTYPES.TYPE_Message:
                                    {
                                        if (NewMessageEvent != null)
                                        {
                                            PACKET_DATA IncomingData = new PACKET_DATA();
                                            IncomingData = (PACKET_DATA)PACKET_FUNCTIONS.ByteArrayToStructure(fp.ThePacket, typeof(PACKET_DATA));
                                            var msg = new string(IncomingData.szStringDataA).TrimEnd('\0');
                                            var user = new string(IncomingData.szStringData150).TrimEnd('\0');
                                            NewMessageEvent(user, msg, IncomingData.Data16);
                                        }
                                    }
                                    break;
                            }

                            if (client != null)
                                client.LastDataFromServer = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            Log.Message(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        public event NewMessageHandler NewMessageEvent;
        public delegate void NewMessageHandler(string user, string message, int data16, bool silent = false);


        private void TellServerImDisconnecting()
        {
            try
            {
                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_Close;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

                SendMessageToServer(byData);
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }

        public event ConnectionStatusHandler ConnectionStatusEvent;
        public delegate void ConnectionStatusHandler(TCPLogic client, EventArgs e, bool ConnectionStatus);

        bool ImDisconnecting = false;
        private void DoServerDisconnect()
        {
            if (ImDisconnecting)
                return;

            ImDisconnecting = true;
            
            try
            {
                int i = 0;


                if (client != null)
                {
                    TellServerImDisconnecting();
                    Thread.Sleep(75);
                }
                

                ServerConnected = false;
                ConnectionStatusEvent(this, EventArgs.Empty, false);

                DestroyGeneralTimer();
                
                try
                {
                    if (autoEventHostServer != null)
                    {
                        autoEventHostServer.Set();

                        i = 0;
                        while (DataProcessHostServerThread.IsAlive)
                        {
                            Thread.Sleep(1);
                            if (i++ > 200)
                            {
                                DataProcessHostServerThread.Abort();
                                break;
                            }
                        }

                        autoEventHostServer.Close();
                        autoEventHostServer.Dispose();
                        autoEventHostServer = null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Message(ex.ToString());
                }
                
                if (autoEvent2 != null)
                {
                    autoEvent2.Set();

                    autoEvent2.Close();
                    autoEvent2.Dispose();
                    autoEvent2 = null;
                }
                
                if (client != null)
                {
                    if (client.OnReceiveData != null)
                        client.OnReceiveData -= OnDataReceive;
                    if (client.OnDisconnected != null)
                        client.OnDisconnected -= OnDisconnect;

                    client.Disconnect();
                    client = null;
                }

            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
            finally
            {
                ImDisconnecting = false;
            }

            return;
        }

        private void DestroyGeneralTimer()
        {
            if (GeneralTimer != null)
            {
                if (GeneralTimer.IsEnabled == true)
                    GeneralTimer.IsEnabled = false;

                try
                {
                    GeneralTimer.Tick -= GeneralTimer_Tick;
                }
                catch (Exception ex)
                {
                    Log.Message(ex.ToString());
                }
                GeneralTimer.Stop();
                GeneralTimer = null;
            }
        }
        private delegate void HostCommunicationsHasQuitDelegate(bool FromHost);
        private void HostCommunicationsHasQuit(bool FromHost)
        {

            if (client != null)
            {
                DoServerDisconnect();
            }
        }

        private void CheckThisComputersTimeAgainstServerTime()
        {
            Int64 timeDiff = DateTime.UtcNow.Ticks - ServerTime;
            TimeSpan ts = TimeSpan.FromTicks(Math.Abs(timeDiff));

            if (ts.TotalMinutes > 15)
            {
                string msg = string.Format("Computer Time Discrepancy!! " +
                    "The time on this computer differs greatly " +
                    "compared to the time on the Realtrac Server " +
                    "computer. Check this PC's time.");
                
                Log.Message(msg);
            }
        }
        private void ReplyToHostPing(byte[] message)
        {
            try
            {
                PACKET_DATA IncomingData = new PACKET_DATA();
                IncomingData = (PACKET_DATA)PACKET_FUNCTIONS.ByteArrayToStructure(message, typeof(PACKET_DATA));
                
                TimeSpan ts = (new DateTime(IncomingData.DataLong1)) - (new DateTime(ServerTime));
                Log.Message($"{string.Format("Ping From Server to client: {0:0.##}ms", ts.TotalMilliseconds)}");

                ServerTime = IncomingData.DataLong1;

                PACKET_DATA xdata = new PACKET_DATA();

                xdata.Packet_Type = (UInt16)PACKETTYPES.TYPE_PingResponse;
                xdata.Data_Type = 0;
                xdata.Packet_Size = 16;
                xdata.maskTo = 0;
                xdata.idTo = 0;
                xdata.idFrom = 0;

                xdata.DataLong1 = IncomingData.DataLong1;

                byte[] byData = PACKET_FUNCTIONS.StructureToByteArray(xdata);

                SendMessageToServer(byData);

                CheckThisComputersTimeAgainstServerTime();
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
            }
        }
    }
}
