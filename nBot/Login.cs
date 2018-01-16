using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace nBot
{
    class Login
    {
        public static void LoginReply(Packet packet)
        {
            byte result = packet.ReadBYTE();
            if (result == 1)
            {
                uint id = packet.ReadDWORD();
                string ip = packet.ReadSTRING("ascii");
                ushort port = packet.ReadWORD();
                
                Agent.xfer_remote_ip = ip;
                Agent.xfer_remote_port = port;
                Packet new_packet = new Packet(0xA102, true);
                new_packet.AddBYTE(result);
                new_packet.AddDWORD(id);
                new_packet.AddSTRING("127.0.0.1", "ascii");
                new_packet.AddWORD((ushort)Globals.ListenPort);
                Thread sss = new Thread(Agent.AgentThread);
                sss.Start();
                Gateway.SendToClient(new_packet);
            }
            else
            { // error
                Gateway.SendToClient(packet);
            }
        }
        public static void ParseServerStats(Packet packet)
        {
            byte new_entry = packet.ReadBYTE();
            while (new_entry == 1)
            {
                byte id = packet.ReadBYTE();
                string _name = packet.ReadSTRING("ascii");
                new_entry = packet.ReadBYTE();
            }
            if (Globals.Main.servers.Enabled == false && Globals.LoginType == Globals.enumLoginType.Clientless)
            {
                Globals.Main.servers.Enabled = true;
                Globals.Main.id.Enabled = true;
                Globals.Main.pw.Enabled = true;
                Globals.Main.btnLogin.Enabled = true;
            }
            new_entry = packet.ReadBYTE();
            while (new_entry == 1)
            {
                ushort id = 0;
                id = packet.ReadWORD();
                string name = packet.ReadSTRING("ascii");
                ushort current = packet.ReadWORD();
                ushort max = packet.ReadWORD();
                byte statu = packet.ReadBYTE();
                new_entry = packet.ReadBYTE();
                Globals.UpdateLogs("[" + name + "] " + current + "/" + max + " " + statu);
                if (Globals.LoginType == Globals.enumLoginType.Clientless)
                {
                    Globals.servers_list.Add(name, id);
                    Globals.Main.servers.Items.Add(name);
                }
            }
        }
        public static void HandleCharList(Packet packet)
        {
            if (packet.ReadBYTE() == 2) // character listening
            {
                if (!Globals.Main.char_list.Enabled)
                {
                    Globals.Main.char_list.Enabled = true;
                    Globals.Main.charSelect.Enabled = true;
                }
                if (packet.ReadBYTE() == 1) // result
                {
                    byte charCount = packet.ReadBYTE();
                    for (int i = 0; i < charCount; i++)
                    {
                        uint CharID = packet.ReadDWORD();
                        string CharName = packet.ReadSTRING("ascii");
                        packet.ReadBYTE();
                        packet.ReadBYTE();
                        packet.ReadQWORD();
                        packet.ReadWORD();
                        packet.ReadWORD();
                        packet.ReadWORD();
                        packet.ReadDWORD();
                        packet.ReadDWORD();

                        byte doDelete = packet.ReadBYTE();
                        if (doDelete == 1)
                            packet.ReadDWORD();

                        packet.ReadWORD();
                        packet.ReadBYTE();
                        byte itemCount = packet.ReadBYTE();

                        for (int y = 0; y < itemCount; y++)
                        {
                            UInt32 item_id = packet.ReadDWORD();
                            byte item_plus = packet.ReadBYTE();
                        }

                        byte Avatars_count = packet.ReadBYTE();
                        for (int y = 0; y < Avatars_count; y++)
                        {
                            UInt32 item_id = packet.ReadDWORD();
                            byte item_plus = packet.ReadBYTE();
                        }
                        Globals.Main.char_list.Items.Add(CharName);
                        Globals.Main.char_list.SelectedItem = CharName;

                    }
                }
            }
        }
    }
}
