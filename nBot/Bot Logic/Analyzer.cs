using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot
{
    class Analyzer
    {
        public static void ClientAnalyze(object e)
        {
            if (Globals.Analyzer.analyzer_enabled.Checked)
            {
                Packet packet;
                packet = (Packet)e;
                byte[] packet_bytes = new byte[8196];
                packet_bytes = packet.GetBytes();
                if (Globals.Analyzer.clienttoserver.Checked || Globals.Analyzer.both.Checked)
                {
                    if (Globals.Analyzer.listen_list.Items.Count > 0)
                    {
                        if (Globals.Analyzer.listen_list.Items.Contains((object)packet.Opcode.ToString("X2")))
                        {
                            Globals.Analyzer.AddPacket("[C -> S] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                        }
                    }
                    else if (Globals.Analyzer.block_list.Items.Count > 0)
                    {
                        if (!Globals.Analyzer.block_list.Items.Contains((object)packet.Opcode.ToString("X2")))
                        {
                            Globals.Analyzer.AddPacket("[C -> S] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                        }
                    }
                    else
                    {
                        Globals.Analyzer.AddPacket("[C -> S] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                    }
                }
            }
        }
        public static void ServerAnalyze(object e)
        {
            if (Globals.Analyzer.analyzer_enabled.Checked)
            {
                Packet packet;
                packet = (Packet)e;
                byte[] packet_bytes = new byte[8196];
                packet_bytes = packet.GetBytes();
                if (Globals.Analyzer.servertoclient.Checked || Globals.Analyzer.both.Checked)
                {
                    if (Globals.Analyzer.listen_list.Items.Count > 0)
                    {
                        if (Globals.Analyzer.listen_list.Items.Contains((object)packet.Opcode.ToString("X2")))
                        {
                            Globals.Analyzer.AddPacket("[S -> C] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                        }
                    }
                    else if (Globals.Analyzer.block_list.Items.Count > 0)
                    {
                        if (!Globals.Analyzer.block_list.Items.Contains((object)packet.Opcode.ToString("X2")))
                        {
                            Globals.Analyzer.AddPacket("[S -> C] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                        }
                    }
                    else
                    {
                        Globals.Analyzer.AddPacket("[S -> C] [" + packet.Opcode.ToString("X2") + "] " + packet_bytes.Length + " Bytes" + Environment.NewLine + Utility.HexDump(packet_bytes));
                    }
                }
            }
        }
    }
}
