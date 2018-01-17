using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nBot.Packets
{
    class Buff
    {
        public static void ParseBuffActivating(Packet packet)
        {
            if (packet.ReadDWORD() == Character.ObjectID)
            {
                uint id = packet.ReadDWORD();
                if (Globals.char_skills.ContainsKey(id))
                {
                    SkillData tempskill = Globals.char_skills[id];
                    uint Temp = packet.ReadDWORD();
                    if (Temp == 0) return;
                    tempskill.TempID = Temp;
                    Globals.char_skills[id] = tempskill;
                    if (Logic.Buff.ActiveBuffs.Values.ToList().Any(x => x.Name == tempskill.Name))
                    {
                       Logic.Buff.ActiveBuffs.Remove(Logic.Buff.ActiveBuffs.Values.ToList().Find(x => x.Name == tempskill.Name).TempID);
                       Globals.Main.activeBuffs.Items.Remove(tempskill.Name);
                    }
                    Logic.Buff.ActiveBuffs.Add(Temp, tempskill);
                    Globals.Main.activeBuffs.Items.Add(tempskill.Name);
                }
            }
        }
        public static void ParseBuffEnd(Packet packet)
        {
            byte count = packet.ReadBYTE();
            for (int i = 0; i < count; i++)
            {
                uint TempID = packet.ReadDWORD();
                RemoveBuff(TempID);
            }
            
        }
        public static void RemoveBuff(uint TempID)
        {
            if (Logic.Buff.ActiveBuffs.ContainsKey(TempID))
            {
                SkillData tempskill = Globals.char_skills.Values.ToList().Find(x => x.TempID == TempID);
                tempskill.TempID = 0;
                Globals.char_skills[Logic.Buff.GetID(tempskill.ID)] = tempskill;
                Globals.Main.activeBuffs.Items.Remove(Logic.Buff.ActiveBuffs[TempID].Name);
                Logic.Buff.ActiveBuffs.Remove(TempID);
            }
        }
    }
}
