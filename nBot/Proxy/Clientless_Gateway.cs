using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace nBot
{
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }

    public class ClientlessGateway
    {
        static Security gw_security = new Security();
        static TransferBuffer gw_recv_buffer = new TransferBuffer(4096, 0, 0);
        static List<Packet> gw_packets = new List<Packet>();
        static Socket gw_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static uint version = UInt32.Parse("6");
        static uint locale = UInt32.Parse("22");
        static Thread loop;
        public void Start(string IP, string Port)
        {
            loop = new Thread(Gateway_thread);
            gw_socket.Connect(IP, Int32.Parse(Port));
            loop.Start();
            gw_socket.Blocking = false;
            gw_socket.NoDelay = true;
        }
        public void Gateway_thread()
        {
            while (true)
            {
                if (!gw_socket.IsConnected())
                {
                    if (!ClientlessAgent.ag_socket.IsConnected())
                        Globals.UpdateLogs("Disconnected from the server.");
                    break;
                }
                SocketError err;
                gw_recv_buffer.Size = gw_socket.Receive(gw_recv_buffer.Buffer, 0, gw_recv_buffer.Buffer.Length, SocketFlags.None, out err);
                if (err != SocketError.Success)
                {
                    if (err != SocketError.WouldBlock)
                    {
                        break;
                    }
                }
                else
                {
                    if (gw_recv_buffer.Size > 0)
                    {
                        gw_security.Recv(gw_recv_buffer);
                    }
                    else
                    {
                        break;
                    }
                }
                List<Packet> tmp_packets = gw_security.TransferIncoming();
                if (tmp_packets != null)
                {
                    gw_packets.AddRange(tmp_packets);
                }

                if (gw_packets.Count > 0)
                {
                    foreach (Packet packet in gw_packets)
                    {
                        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000)
                        {
                            continue;
                        }

                        else if (packet.Opcode == 0xa100)
                        {
                            Packet packet2 = new Packet(0x6106, false, true);
                            packet2.WriteUInt8((byte)1);
                            gw_security.Send(packet2);
                            Packet packet3 = new Packet(0x6102, false, true);
                            packet3.WriteUInt8((byte)1);
                            gw_security.Send(packet3);
                            gw_security.Send(packet3);
                        }
                        else if (packet.Opcode == 0x6005)
                        {
                            Packet packet4 = new Packet(0x2002, false);
                            gw_security.Send(packet4);
                            Packet packet5 = new Packet(0x6102, true);
                            gw_security.Send(packet5);
                        }
                        
                       /* else if (packet.Opcode == 0x2001)
                        {
                            if (packet.ReadSTRING("ascii") == "GatewayServer")
                            {
                                Globals.Server = Globals.ServerEnum.Gateway;
                                Packet response = new Packet(0x6100, true, false);
                                response.WriteUInt8(locale);
                                response.WriteAscii("SR_Client");
                                response.WriteUInt32(version);
                                gw_security.Send(response);
                            }
                        }*/

                        else if (packet.Opcode == 0xA323)
                        {
                            if (packet.ReadBYTE() == 1)
                            {
                                Globals.Main.pic.Image.Dispose();
                                Globals.Main.pic.Image = null;
                                //System.IO.File.Delete(Captcha.lastImageName);
                            }
                            else
                            {
                                Globals.Main.pic.Image.Dispose();
                                Globals.Main.pic.Image = null;
                                //System.IO.File.Delete(Captcha.lastImageName);
                                Globals.Main.btnLogin.PerformClick();
                            }
                        }
                       /* else if (packet.Opcode == 0xA100)
                        {
                            byte result = packet.ReadBYTE();
                            if (result == 1)
                            {
                                Packet response = new Packet(0x6101, true);
                                gw_security.Send(response);
                            }
                            else
                            {
                                return;
                            }

                        }*/
                        else if (packet.Opcode == 0xA102)
                        {
                            if (packet.ReadBYTE() == 1)
                            {
                                uint LoginID = packet.ReadDWORD();
                                string ip = packet.ReadSTRING("ascii");
                                ushort port = packet.ReadWORD();
                                ClientlessAgent ag = new ClientlessAgent();
                                ag.Start(ip, port.ToString(), LoginID, Globals.Main.id.Text, Globals.Main.pw.Text);
                            }
                            else
                            {
                                byte error = packet.ReadBYTE();
                                MessageBox.Show("errorlar:" + error);
                            }
                        }
                        GatewayHandler.RemotePacketHandler(packet);
                    }
                    gw_packets.Clear();
                }
                List<KeyValuePair<TransferBuffer, Packet>> tmp_buffers = gw_security.TransferOutgoing();
                if (tmp_buffers != null)
                {
                    foreach (var kvp in tmp_buffers)
                    {
                        TransferBuffer buffer = kvp.Key;
                        Packet packet = kvp.Value;
                        err = SocketError.Success;
                        while (buffer.Offset != buffer.Size)
                        {

                            int sent = gw_socket.Send(buffer.Buffer, buffer.Offset, buffer.Size - buffer.Offset, SocketFlags.None, out err);
                            Analyzer.ClientAnalyze(packet);
                            if (err != SocketError.Success)
                            {
                                if (err != SocketError.WouldBlock)
                                {
                                    break;
                                }
                            }


                            buffer.Offset += sent;
                            Thread.Sleep(1);
                        }
                        if (err != SocketError.Success)
                        {
                            break;
                        }

                    }

                    if (err != SocketError.Success)
                    {
                        break;
                    }
                }
                Thread.Sleep(1);
            }
        }
        public static void Send(Packet packet)
        {
            gw_security.Send(packet);
        }
    }
}
