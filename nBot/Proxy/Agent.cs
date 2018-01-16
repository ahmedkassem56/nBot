using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace nBot
{
    class Agent
    {
        public static TcpListener ag_local_server;
        public static TcpClient ag_local_client;
        public static TcpClient ag_remote_client;
        public static Security ag_local_security;
        public static Security ag_remote_security;
        public static NetworkStream ag_local_stream;
        public static NetworkStream ag_remote_stream;
        public static TransferBuffer ag_remote_recv_buffer;
        public static List<Packet> ag_remote_recv_packets;
        public static List<KeyValuePair<TransferBuffer, Packet>> ag_remote_send_buffers;
        public static TransferBuffer ag_local_recv_buffer;
        public static List<Packet> ag_local_recv_packets;
        public static List<KeyValuePair<TransferBuffer, Packet>> ag_local_send_buffers;
        public static Thread remote_thread;
        public static Thread local_thread;
        public static string xfer_remote_ip;
        public static int xfer_remote_port;
         
        static void AgentRemoteThread()
        {
            try
            {
                while (true)
                {

                    if (ag_remote_stream.DataAvailable)
                    {
                        ag_remote_recv_buffer.Offset = 0;
                        ag_remote_recv_buffer.Size = ag_remote_stream.Read(ag_remote_recv_buffer.Buffer, 0, ag_remote_recv_buffer.Buffer.Length);
                        ag_remote_security.Recv(ag_remote_recv_buffer);
                    }

                    ag_remote_recv_packets = ag_remote_security.TransferIncoming();
                    if (ag_remote_recv_packets != null)
                    {
                        foreach (Packet packet in ag_remote_recv_packets)
                        {
                            byte[] packet_bytes = packet.GetBytes();
                           // Console.WriteLine("[S->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);
                            AgentHandler.RemotePacketHandler(packet);

                        }
                    }

                    ag_remote_send_buffers = ag_remote_security.TransferOutgoing();
                    if (ag_remote_send_buffers != null)
                    {
                        foreach (var kvp in ag_remote_send_buffers)
                        {
                            Packet packet = kvp.Value;
                            TransferBuffer buffer = kvp.Key;

                            byte[] packet_bytes = packet.GetBytes();
                            //Console.WriteLine("[P->S][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);

                            

                            ag_remote_stream.Write(buffer.Buffer, 0, buffer.Size);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch 
            {
                //Console.WriteLine("[AgentRemoteThread] Exception: {0}", ex);
            }
        }
        static void AgentLocalThread()
        {
            try
            {
                while (true)
                {

                    if (Globals.LoginType == Globals.enumLoginType.Clientless2)
                    {
                        ag_local_client.Close();
                        ag_local_client = null;
                    }
                    if (Globals.LoginType == Globals.enumLoginType.Client)
                    {
                        if (ag_local_stream.DataAvailable)
                        {
                            ag_local_recv_buffer.Offset = 0;
                            ag_local_recv_buffer.Size = ag_local_stream.Read(ag_local_recv_buffer.Buffer, 0, ag_local_recv_buffer.Buffer.Length);
                            ag_local_security.Recv(ag_local_recv_buffer);
                        }

                        ag_local_recv_packets = ag_local_security.TransferIncoming();
                        if (ag_local_recv_packets != null)
                        {
                            foreach (Packet packet in ag_local_recv_packets)
                            {
                                byte[] packet_bytes = packet.GetBytes();
                                //Console.WriteLine("[C->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);

                                // Do not pass through these packets.
                                AgentHandler.LocalPacketHandler(packet);
                            }
                        }
                    }
                    ag_local_send_buffers = ag_local_security.TransferOutgoing();
                    if (ag_local_send_buffers != null)
                    {
                        foreach (var kvp in ag_local_send_buffers)
                        {
                            Packet packet = kvp.Value;
                            TransferBuffer buffer = kvp.Key;

                            byte[] packet_bytes = packet.GetBytes();
                            //Console.WriteLine("[P->C][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);

                            ag_local_stream.Write(buffer.Buffer, 0, buffer.Size);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[AgentLocalThread] Exception: {0}", ex);
            }
        }
        public static void AgentThread()
        {
            try
            {
                ag_local_security = new Security();
                ag_local_security.GenerateSecurity(true, true, true);
                ag_remote_security = new Security();
                ag_remote_recv_buffer = new TransferBuffer(4096, 0, 0);
                ag_local_recv_buffer = new TransferBuffer(4096, 0, 0);
                ag_local_server = new TcpListener(IPAddress.Parse("127.0.0.1"), Globals.ListenPort);
                ag_local_server.Start();
                ag_local_client = ag_local_server.AcceptTcpClient();
                ag_remote_client = new TcpClient();
                ag_local_server.Stop();
                ag_remote_client.Connect(xfer_remote_ip, xfer_remote_port);
                ag_local_stream = ag_local_client.GetStream();
                ag_remote_stream = ag_remote_client.GetStream();
                remote_thread = new Thread(AgentRemoteThread);
                remote_thread.Start();
                local_thread = new Thread(AgentLocalThread);
                local_thread.Start();
                remote_thread.Join();
                local_thread.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AgentThread] Exception: {0}", ex);
            }
        }
        public static void SendToServer(Packet packet)
        {
            if (Globals.LoginType == Globals.enumLoginType.Client || Globals.LoginType == Globals.enumLoginType.Clientless2)
                ag_remote_security.Send(packet);
            else
                ClientlessAgent.Send(packet);
        }
        public static void SendToClient(Packet packet)
        {
            if (Globals.LoginType == Globals.enumLoginType.Client)
                ag_local_security.Send(packet);
        }
        

    }
}
