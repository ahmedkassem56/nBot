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
    public class ClientlessAgent
    {
        static Security ag_security = new Security();
        static TransferBuffer ag_recv_buffer = new TransferBuffer(4096, 0, 0);
        static List<Packet> ag_packets = new List<Packet>();
        public static Socket ag_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static uint version = UInt32.Parse("6");
        static uint locale = UInt32.Parse("22");
        static Thread loop;
        static uint loginID;
        static string username;
        static string password;
        public void Start(string IP, string Port, uint _loginID, string _username, string _password)
        {
            loginID = _loginID;
            username = _username;
            password = _password;
            loop = new Thread(Agent_thread);
            ag_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ag_socket.Connect(IP, Int32.Parse(Port));
            loop.Start();
            ag_socket.Blocking = false;
            ag_socket.NoDelay = true;
        }
        public void Agent_thread()
        {
            while (true)
            {
                if (!ag_socket.IsConnected())
                {
                    Globals.UpdateLogs("Disconnected from the server.");
                    break;
                }
                SocketError err;
                ag_recv_buffer.Size = ag_socket.Receive(ag_recv_buffer.Buffer, 0, ag_recv_buffer.Buffer.Length, SocketFlags.None, out err);
                if (err != SocketError.Success)
                {
                    if (err != SocketError.WouldBlock)
                    {
                        break;
                    }
                }
                else
                {
                    if (ag_recv_buffer.Size > 0)
                    {
                        ag_security.Recv(ag_recv_buffer);
                    }
                    else
                    {
                        break;
                    }
                }

                // Obtain all queued packets and add them to our own queue to process later.
                List<Packet> tmp_packets = ag_security.TransferIncoming();
                if (tmp_packets != null)
                {
                    ag_packets.AddRange(tmp_packets);
                }

                if (ag_packets.Count > 0)
                {
                    foreach (Packet packet in ag_packets)
                    {
                        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000)
                        {
                            continue;
                        }
                        if (packet.Opcode == 0x2001)
                        {
                            if (packet.ReadSTRING("ascii") == "GatewayServer")
                            {
                                Globals.Server = Globals.ServerEnum.Gateway;
                                Packet response = new Packet(0x6100, true, false);
                                response.AddBYTE((byte)locale);
                                response.AddSTRING("SR_Client","ascii");
                                response.AddDWORD(version);
                                ag_security.Send(response);
                            }
                            else
                            {
                                Globals.Server = Globals.ServerEnum.Agent;
                                Packet p = new Packet(0x6103);
                                p.AddDWORD(loginID);
                                p.WriteAscii(username);
                                p.WriteAscii(password);
                                p.AddBYTE(22);
                                p.AddDWORD(0);
                                p.AddWORD(0);
                                ag_security.Send(p);
                            }
                        }
                        else if (packet.Opcode == 0xA103)
                        {
                            byte flag = packet.ReadBYTE();
                            if (flag == 1)
                            {
                                Packet response = new Packet(0x7007);
                                response.AddBYTE(2);
                                ag_security.Send(response);
                            }
                            else if (flag == 2)
                            {
                                if (packet.ReadBYTE() == 4)
                                {
                                    Globals.UpdateLogs("Server is full");
                                    Globals.Main.btnLogin.Enabled = true;
                                }
                            }
                        }
                        else if (packet.Opcode == 0xB007)
                        {
                            Login.HandleCharList(packet);
                        }
                        else if (packet.Opcode == 0x3020)
                        {
                            Packet p = new Packet(0x3012);
                            Send(p);
                        }
                        AgentHandler.RemotePacketHandler(packet);
                    }
                    ag_packets.Clear();
                }
                List<KeyValuePair<TransferBuffer, Packet>> tmp_buffers = ag_security.TransferOutgoing();
                if (tmp_buffers != null)
                {
                    foreach (var kvp in tmp_buffers)
                    {
                        TransferBuffer buffer = kvp.Key;
                        Packet packet = kvp.Value;
                        err = SocketError.Success;
                        while (buffer.Offset != buffer.Size)
                        {
                            int sent = ag_socket.Send(buffer.Buffer, buffer.Offset, buffer.Size - buffer.Offset, SocketFlags.None, out err);
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
        public void Log(string msg, params object[] values)
        {
            msg = string.Format(msg, values);
            Globals.Main.logs.AppendText(msg + "\n");
        }
        public static void Send(Packet packet)
        {
            ag_security.Send(packet);
        }
    }
}
