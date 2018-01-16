using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
namespace nBot
{
    class AgentHandler
    {
        public static void RemotePacketHandler(Packet packet)  // Agent server S -> C packet handler
        {
            Analyzer.ServerAnalyze(packet);
            switch (packet.Opcode)
            {
                case 0x5000:
                case 0x9000:
                    break;
                case 0x303D:
                    Packets.CharData.ParseCharStats(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0x3013:
                    //Packets.CharData.ParseCharData(packet);
                    Packets.CharData.char_data = packet;
                    Agent.SendToClient(packet);
                    break;
                case 0x3020:
                    byte[] packet_bytes = packet.GetBytes();
                    Character.CharID = new byte[] { packet_bytes[0], packet_bytes[1], packet_bytes[2], packet_bytes[3] };
                    Packets.CharData.ParseCharData(Packets.CharData.char_data);
                    Character.ObjectID = packet.ReadDWORD();
                    Agent.SendToClient(packet);
                    break;
                case 0x304E:
                    byte flag = packet.ReadBYTE();
                    if (flag == 4) // zerk points
                    {
                        Character.ZerkPoints = packet.ReadBYTE();
                    }
                    else if (flag == 2) // sp
                    {
                        Character.sp = packet.ReadDWORD();
                    }
                    else if (flag == 1) // gold
                    {
                        Character.gold = packet.ReadQWORD();
                    }
                    Agent.SendToClient(packet);
                    break;
                case 0x3057:
                    Packets.Status.ParseHPMPUpdate(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0x3056:
                    Packets.Status.EXPUpdate(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB021:
                    Packets.Status.ParseMovement(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0x3015:
                    //Globals.UpdateLogs(string.Format("[3015]\n{0}", Utility.HexDump(packet.GetBytes())));
                    Packets.Spawn.SingleSpawn(packet);
                  //  Globals.UpdateLogs(string.Format("[S->C][{0:X4}]{1}{2}{3}{4}{5}", packet.Opcode, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet.GetBytes()), Environment.NewLine));
                    Agent.SendToClient(packet);
                    break;
                case 0x3019:
                    //Globals.UpdateLogs(string.Format("[3019]\n{0}", Utility.HexDump(packet.GetBytes())));
                    Packets.Spawn.GroupSpawn(packet);
                   // Globals.UpdateLogs(string.Format("[S->C][{0:X4}]{1}{2}{3}{4}{5}", packet.Opcode, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet.GetBytes()), Environment.NewLine));
                    Agent.SendToClient(packet);
                    break;
                case 0x3017:
                   // Globals.UpdateLogs(string.Format("[3017]\n{0}", Utility.HexDump(packet.GetBytes())));
                    Packets.Spawn.GroupSpawn_Start(packet);
                   // Globals.UpdateLogs(string.Format("[S->C][{0:X4}]{1}{2}{3}{4}{5}", packet.Opcode, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet.GetBytes()), Environment.NewLine));

                    Agent.SendToClient(packet);
                    break;
                case 0x3018:
                    //Globals.UpdateLogs(string.Format("[3018]\n{0}", Utility.HexDump(packet.GetBytes())));
                    Packets.Spawn.GroupSpawnEnd();
                   // Globals.UpdateLogs(string.Format("[S->C][{0:X4}]{1}{2}{3}{4}{5}", packet.Opcode, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, Utility.HexDump(packet.GetBytes()), Environment.NewLine));

                    Agent.SendToClient(packet);
                    break;
                case 0x3016:
                    Packets.Spawn.SingleDespawn(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB070:
                    Logic.Attack.HandleSkillCasting(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB0BD:
                    Packets.Buff.ParseBuffActivating(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB072:
                    Packets.Buff.ParseBuffEnd(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB045:
                    Logic.Attack.HandleSelectResponse(packet);
                    Agent.SendToClient(packet);
                    break;
                case 0xB074:
                    Logic.Attack.HandleCastSkillResponse(packet);
                    Agent.SendToClient(packet);
                    break;
                default:
                    Agent.SendToClient(packet);
                    break;
            }
        }


        public static void LocalPacketHandler(Packet packet) // Agent server C -> S handler
        {
            Analyzer.ClientAnalyze(packet);
            switch (packet.Opcode)
            {
                case 0x5000:
                case 0x9000:
                case 0x2001:
                    break;
                default:
                    Agent.SendToServer(packet);
                    break;
            }
        }


    }
}
