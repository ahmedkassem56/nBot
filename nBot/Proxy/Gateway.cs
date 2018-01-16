using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace nBot
{
    class Gateway
    {
            
        public static TcpListener gw_local_server;
        public static TcpClient gw_local_client;
        public static TcpClient gw_remote_client;
        public static Security gw_local_security;
        public static Security gw_remote_security;
        public static NetworkStream gw_local_stream;
        public static NetworkStream gw_remote_stream;
        public static TransferBuffer gw_remote_recv_buffer;
        public static List<Packet> gw_remote_recv_packets;
        public static List<KeyValuePair<TransferBuffer, Packet>> gw_remote_send_buffers;
        public static TransferBuffer gw_local_recv_buffer;
        public static List<Packet> gw_local_recv_packets;
        public static List<KeyValuePair<TransferBuffer, Packet>> gw_local_send_buffers;
        public static Thread remote_thread;
        public static Thread local_thread;

        public static void GatewayRemoteThread()
        {
            try
            {
                while (true)
                {

                    if (gw_remote_stream.DataAvailable)
                    {
                        gw_remote_recv_buffer.Offset = 0;
                        gw_remote_recv_buffer.Size = gw_remote_stream.Read(gw_remote_recv_buffer.Buffer, 0, gw_remote_recv_buffer.Buffer.Length);
                        gw_remote_security.Recv(gw_remote_recv_buffer);
                    }

                    gw_remote_recv_packets = gw_remote_security.TransferIncoming();
                    if (gw_remote_recv_packets != null)
                    {
                        foreach (Packet packet in gw_remote_recv_packets)
                        {
                            byte[] packet_bytes = packet.GetBytes();
                            GatewayHandler.RemotePacketHandler(packet);
                        }
                    }

                    gw_remote_send_buffers = gw_remote_security.TransferOutgoing();
                    if (gw_remote_send_buffers != null)
                    {
                        foreach (var kvp in gw_remote_send_buffers)
                        {
                            Packet packet = kvp.Value;
                            TransferBuffer buffer = kvp.Key;

                            byte[] packet_bytes = packet.GetBytes();
                            //Console.WriteLine("[P->S][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);


                            gw_remote_stream.Write(buffer.Buffer, 0, buffer.Size);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch
            {

            }
        }
        public static void GatewayLocalThread()
        {
            try
            {
                while (true)
                {
                    if (Globals.LoginType == Globals.enumLoginType.Clientless2)
                    {
                        gw_local_client.Close();
                        gw_local_client = null;
                    }
                    if (Globals.LoginType == Globals.enumLoginType.Client)
                    {
                        if (gw_local_stream.DataAvailable)
                        {
                            gw_local_recv_buffer.Offset = 0;
                            gw_local_recv_buffer.Size = gw_local_stream.Read(gw_local_recv_buffer.Buffer, 0, gw_local_recv_buffer.Buffer.Length);
                            gw_local_security.Recv(gw_local_recv_buffer);
                        }

                        gw_local_recv_packets = gw_local_security.TransferIncoming();
                        if (gw_local_recv_packets != null)
                        {
                            foreach (Packet packet in gw_local_recv_packets)
                            {
                                byte[] packet_bytes = packet.GetBytes();
                                //Console.WriteLine("[C->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);

                                // Do not pass through these packets.
                                GatewayHandler.LocalPacketHandler(packet);
                            }
                        }
                    }
                    gw_local_send_buffers = gw_local_security.TransferOutgoing();
                    if (gw_local_send_buffers != null)
                    {
                        foreach (var kvp in gw_local_send_buffers)
                        {
                            Packet packet = kvp.Value;
                            TransferBuffer buffer = kvp.Key;

                            byte[] packet_bytes = packet.GetBytes();
                            //Console.WriteLine("[P->C][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet_bytes), Environment.NewLine);

                            gw_local_stream.Write(buffer.Buffer, 0, buffer.Size);
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[GatewayLocalThread] Exception: {0}", ex);
            }
        }
        public static void SendToClient(Packet packet)
        {
            if (Globals.LoginType == Globals.enumLoginType.Client)
                gw_local_security.Send(packet);
        }
        public static void SendToServer(Packet packet)
        {
            if (Globals.LoginType == Globals.enumLoginType.Client || Globals.LoginType == Globals.enumLoginType.Clientless2)
                gw_remote_security.Send(packet);
            else
                ClientlessGateway.Send(packet);
        }

        public static void GatewayThread()
        {
            try
            {
                gw_local_security = new Security();
                gw_local_security.GenerateSecurity(true, true, true);
                gw_remote_security = new Security();
                gw_remote_recv_buffer = new TransferBuffer(4096, 0, 0);
                gw_local_recv_buffer = new TransferBuffer(4096, 0, 0);
                gw_local_server = new TcpListener(IPAddress.Parse("127.0.0.1"), Globals.ListenPort);
                gw_local_server.Start();
                gw_local_client = gw_local_server.AcceptTcpClient();
                gw_remote_client = new TcpClient();
                gw_local_server.Stop();
                gw_remote_client.Connect(Globals.IP, 40510);
                gw_local_stream = gw_local_client.GetStream();
                gw_remote_stream = gw_remote_client.GetStream();
                remote_thread = new Thread(GatewayRemoteThread);
                remote_thread.Start();
                local_thread = new Thread(GatewayLocalThread);
                local_thread.Start();
                remote_thread.Join();
                local_thread.Join();
            }
            catch (Exception ex)
            {
                throw new Exception ("[GatewayThread] Exception: {0}", ex);
            }
        }
        public static void ReConnect()
        {
            try
            {
                remote_thread.Abort();
                gw_remote_client.Close();
                Thread.Sleep(15000);
                gw_remote_security = new Security();
                gw_remote_recv_buffer = new TransferBuffer(4096, 0, 0);
                gw_remote_client = new TcpClient();
                gw_remote_client.Connect(Globals.IP, 15779);
                gw_remote_stream = gw_remote_client.GetStream();
                remote_thread = new Thread(GatewayRemoteThread);
                remote_thread.Start();
                remote_thread.Join();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Reconnect Error" + Environment.NewLine + ex.ToString());
            }
        }

    }

}
