using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot.Packets
{
    class Status
    {
        public static void ParseHPMPUpdate(Packet packet)
        {
            uint objectID = packet.ReadDWORD();
            if (objectID == Character.ObjectID)
            {
                byte type1 = packet.ReadBYTE();
                packet.ReadBYTE();
                byte type2 = packet.ReadBYTE();
                if (type2 == 0x01)
                {
                    Character.hp = packet.ReadDWORD();
                }
                else if (type2 == 0x02)
                {
                    Character.mp = packet.ReadDWORD();
                }
                else if (type2 == 0x03)
                {
                    Character.hp = packet.ReadDWORD();
                    Character.mp = packet.ReadDWORD();
                }
            }
            else
            {
                if (Globals.AroundMobs.ContainsKey(objectID))
                {
                    byte type1 = packet.ReadBYTE();
                    packet.ReadBYTE();
                    byte type2 = packet.ReadBYTE();
                    if (type2 == 0x01)
                    {
                        if (packet.ReadDWORD() == 0)
                        {
                            Globals.AroundMobs.Remove(objectID);
                        }
                    }
                    else if (type2 == 0x02)
                    {

                    }
                    else if (type2 == 0x05)
                    {
                        uint hp = packet.ReadDWORD();
                        if (hp == 0)
                        {
                            Globals.AroundMobs.Remove(objectID);
                        }
                    }
                }
            }
        }
        public static void EXPUpdate(Packet packet)
        {
            packet.ReadDWORD(); // mob id
            Character.exp += packet.ReadQWORD();
        }
        public static void ParseMovement(Packet packet)
        {
            uint id = packet.ReadDWORD();
            if (id == Character.ObjectID)
            {
                if (packet.ReadBYTE() == 0x01)
                {
                    byte xsec = packet.ReadBYTE();
                    byte ysec = packet.ReadBYTE();
                    ushort xpos = packet.ReadWORD();
                    ushort zpos = packet.ReadWORD();
                    ushort ypos = packet.ReadWORD();
                    Character.x = Globals.GameX(int.Parse(xsec.ToString()), int.Parse(xpos.ToString()));
                    Character.y = Globals.GameY(int.Parse(ysec.ToString()), int.Parse(ypos.ToString()));
                }
            }
            else if (Globals.AroundMobs.ContainsKey(id))
            {
                if (packet.ReadBYTE() == 0x01)
                {
                    byte xsec = packet.ReadBYTE();
                    byte ysec = packet.ReadBYTE();
                    float xpos = packet.ReadWORD();
                    float zpos = packet.ReadWORD();
                    float ypos = packet.ReadWORD();
                    int real_xpos = 0;
                    int real_ypos = 0;
                    if (xpos > 32768)
                    {
                        real_xpos = (int)(65536 - xpos);
                    }
                    else
                    {
                        real_xpos = (int)xpos;
                    }
                    if (ypos > 32768)
                    {
                        real_ypos = (int)(65536 - ypos);
                    }
                    else
                    {
                        real_ypos = (int)ypos;
                    }
                    Monster temp = Globals.AroundMobs[id];
                    temp.x = Globals.GameX(xsec, real_xpos);
                    temp.y = Globals.GameY(ysec, real_ypos);
                    Globals.AroundMobs[id] = temp;
                }
            }
        }
    }
}
